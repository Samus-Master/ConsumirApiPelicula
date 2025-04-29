using ConsumirApiPelicula.Servicios;
using ConsumirApiPelicula.Servicios.IServicios;
using PiscinaTropical.Utilidades.Servicios;
using PiscinaTropical.Utilidades.Servicios.IServicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//add repositories
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddHttpClient<IPeliculaService, PeliculaService>();
builder.Services.AddScoped<IPeliculaService, PeliculaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ISliderService, SliderService>();


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
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
