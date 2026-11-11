using System.Text;
using System.Text.Json.Serialization;
using EmpresaKenxon.Interfaces;
using EmpresaKenxon.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// se envían como texto ("Parcial") y no como número (0).
builder.Services.AddControllers().AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// se crea una sola instancia para toda la API,
// así todos los controladores comparten la misma lista en memoria.
string carpetaData = Path.Combine(builder.Environment.ContentRootPath, "Data");
builder.Services.AddSingleton<IUsuarioRepositorio>(new UsuarioRepositorio(Path.Combine(carpetaData, "usuarios.json")));
builder.Services.AddSingleton<IClienteRepositorio>(new ClienteRepositorio(Path.Combine(carpetaData, "clientes.json")));
builder.Services.AddSingleton<ICompraRepositorio>(new CompraRepositorio(Path.Combine(carpetaData, "compras.json")));
builder.Services.AddSingleton<IAbonoRepositorio>(new AbonoRepositorio(Path.Combine(carpetaData, "abonos.json")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],// es el destinatario del token, la API.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Clave"])) 
        };
    });// Se valida el token con la clave  que se usó para firmarlo.

builder.Services.AddAuthorization();

// Swagger con el botón "Authorize" para pegar el token.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo { Title = "API Kenxon Car Sound", Version = "v1" });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue aquí solo el token (sin escribir la palabra Bearer)."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {// Esto indica que la API requiere autenticación con JWT.
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Frontend
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

// saber QUIÉN es (autenticación) y luego QUÉ puede hacer (autorización).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
