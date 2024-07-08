using System.Net;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
//builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.WebHost.ConfigureKestrel(ks =>
{
    ks.Listen(IPAddress.Any, 8080);
    ks.Listen(IPAddress.Any, 8081, option =>
    {
        option.UseHttps();
    });
}).UseUrls("http://*:8080","https://*:8081");

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<Service>(options=>
    {
        options.BaseAddresses.Add(new Uri("https://localhost:8081"));
        options.BaseAddresses.Add(new Uri("http://localhost:8080"));
    });
    serviceBuilder.AddServiceEndpoint<Service, IService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/Service.svc");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.Run();
