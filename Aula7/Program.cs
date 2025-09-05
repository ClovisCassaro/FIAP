using dotnet_docs.Infra;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region [Swagger]
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "Minha API",
//        Version = "v1",
//        Description = "Esta é uma API exemplo para demonstrar Swagger",
//        Contact = new Microsoft.OpenApi.Models.OpenApiContact
//        {
//            Name = "Thiago S Adriano",
//            Email = "tadriano@fiap.br.com",
//            Url = new Uri("https://seusite.com")
//        },
//        License = new Microsoft.OpenApi.Models.OpenApiLicense
//        {
//            Name = "MIT",
//            Url = new Uri("https://opensource.org/licenses/MIT")
//        }
//    });

//    c.SchemaFilter<EnumSchemaFilter>();
//});
#endregion

#region [Swagger 2]
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' [espaço] e o token JWT",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.SchemaFilter<EnumSchemaFilter>(); // Manter o filtro existente
});
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDOC API Documentation";
        c.SpecUrl = "/swagger/v1/swagger.json";
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
