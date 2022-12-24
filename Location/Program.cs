using Location.Middleware;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Services.IpLocation;
using Services.IpLocation.Concrete;

var builder = WebApplication.CreateBuilder(args);

//adding MVC to the web-application
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();

//adding session capabilities to the web-application
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//adding the dependency injections
builder.Services.AddSingleton<ILocationService, IpInfoDb>(x => new IpInfoDb("INSERT YOUR KEY HERE", x.GetRequiredService<IMemoryCache>()));
builder.Services.AddSingleton<ILocationService, IpApi>();

var app = builder.Build();

app.UseSession();
app.UseMiddleware<LocationMiddleware>();
app.MapGet("/", (HttpContext context) =>
{
    var json = context.Session.GetString("ip-geolocation");
    var location = JsonConvert.DeserializeObject<LocationModel>(json);
    return $"You location is {location.City} - {location.Country} ({location.CountryCode}).";
});
app.Run();
