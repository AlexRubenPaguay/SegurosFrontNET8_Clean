using SegurosFrontNET8_Clean.Services;

var builder = WebApplication.CreateBuilder(args);

var url = builder.Configuration.GetSection("ApiSettings");
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(url["BaseURL"]);
});
builder.Services.AddScoped<ServiceCliente>();
builder.Services.AddScoped<ServiceSeguro>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
