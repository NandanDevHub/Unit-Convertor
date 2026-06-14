using UnitConversionAPI.Core.Interfaces;
using UnitConversionAPI.Web.Data;
using UnitConversionAPI.Web.Middleware;
using UnitConversionAPI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "Unit Conversion API",
        Version     = "v1",
        Description = "RESTful API for converting numeric values between units of measurement."
    });
});

builder.Services.AddSingleton<IUnitRegistry, UnitRegistry>();
builder.Services.AddScoped<IConversionService, ConversionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();

// Exposed for integration testing
public partial class Program { }
