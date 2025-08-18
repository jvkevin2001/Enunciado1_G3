using Proyecto_API.Services; // Cambia al namespace de tus servicios
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Inyectar tu servicio utilitario
builder.Services.AddScoped<IUtilitarios, Utilitarios>();

// Configurar Swagger con título y descripción
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Proyecto_API",
        Description = "API de ejemplo con JWT",
    });
});

// Obtener la llave secreta para firmar JWT
var llaveSegura = builder.Configuration["Start:LlaveSegura"] ?? throw new Exception("Falta la llave segura en la configuración");

// Configurar JWT
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llaveSegura)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero // No tolerancia extra, expira exactamente en el tiempo
    };
    opt.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var _utilitarios = context.HttpContext.RequestServices.GetRequiredService<IUtilitarios>();
            var respuesta = _utilitarios.RespuestaIncorrecta("JWTNoValido");
            var result = JsonSerializer.Serialize(respuesta);

            return context.Response.WriteAsync(result);
        }
    };
});

var app = builder.Build();

// Configuración de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/api/Error/CapturarError"); // Manejo global de errores
app.UseHttpsRedirection();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
