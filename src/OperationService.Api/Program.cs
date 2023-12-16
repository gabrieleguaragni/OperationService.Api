using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OperationService.Api.Validators;
using OperationService.Business.Abstractions.Kafka;
using OperationService.Business.Abstractions.Services;
using OperationService.Business.Exceptions;
using OperationService.Business.Kafka;
using OperationService.Business.Services;
using OperationService.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient("AuthApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthApiClient:BaseAddress"]!);
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add configuration for DB
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Kafka
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// Add Services
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowerService, FollowerService>();

// Register validators
builder.Services.AddScoped<IValidator<IFormFile>, ImageValidator>();
builder.Services.AddScoped<IValidator<string>, TextValidator>();


builder.Services.AddCors(options =>
{
    // Default policy 
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = $"http://{httpReq.Host.Value}/operation-api" }
        });
    });
    app.UseSwaggerUI();
}

// Global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();