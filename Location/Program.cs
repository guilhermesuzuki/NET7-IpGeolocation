using Location.Facades;
using Location.Interfaces;
using Location.Middlewares;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Services.IpLocation;
using Services.IpLocation.Concrete;

var builder = WebApplication.CreateBuilder(args);

//adding MVC to the web-application
builder.Services.AddMvc();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddSingleton<IUserFacade, UserFacade>();

var app = builder.Build();

app.UseSession();
app.UseMiddleware<LocationMiddleware>();
app.MapGet("/", (HttpContext context, IUserFacade userFacade) =>
{
    var location = userFacade.Location;

    return 
        location != null ?
        $"You location is {location.City} - {location.Country} ({location.CountryCode})." :
        "No user location.";
});
app.Run();
