using MCStore.Business.IServices;
using MCStore.Business.Services;
using MCStore.DAL.Context;
using MCStore.DAL.IRepositories;
using MCStore.DAL.Repositories;
using MCStore.Middlewares.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowEnpoints",
        policy =>
        {
            policy.WithOrigins("https://192.168.100.71",
               "http://192.168.100.71:81",
               "http://localhost:3000",
               "http://localhost:4200",
               "https://purple-coast-044d0bc00.7.azurestaticapps.net")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.MaxDepth = 64;
        });

//Configure authentication and authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}";
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditingSaveChangesInterceptor>();
builder.Services.AddMemoryCache();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MCStoreDbContext>((serviceProvider, options) =>
                                options.UseSqlServer(builder.Configuration.GetConnectionString("MCStoreDbContext"),
                                       sqlOptions =>
                                       {
                                           sqlOptions.EnableRetryOnFailure(
                                               maxRetryCount: 1,
                                               maxRetryDelay: TimeSpan.FromSeconds(30),
                                               errorNumbersToAdd: null);
                                       }
                                    )
                                    .AddInterceptors(serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>())
                                    .LogTo(Console.WriteLine, LogLevel.Error)
                                    .EnableSensitiveDataLogging()
                                );

// register repositories and services for DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IWalletBalanceRepository, WalletBalanceRepository>();
builder.Services.AddScoped<IWalletBalanceService, WalletBalanceService>();
builder.Services.AddScoped<ICashInCashOutFeeRepository, CashInCashOutFeeRepository>();
builder.Services.AddScoped<ICashInCashOutFeeService, CashInCashOutFeeService>();
builder.Services.AddScoped<IReportsRepository, ReportsRepository>();
builder.Services.AddScoped<IReportsService, ReportsService>();

// Ensure IIS integration
builder.WebHost.UseIIS();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    //app.UseExceptionHandler("/Error");
    //app.UseHsts();

    // Global exception handler that returns JSON error info (safe to limit details in production)
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var feature = context.Features.Get<IExceptionHandlerFeature>();
            if (feature?.Error != null)
            {
                var payload = new
                {
                    Message = feature.Error.Message,
#if DEBUG
                    Detail = feature.Error.StackTrace
#endif
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
            else
            {
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { Message = "An unexpected error occurred." }));
            }
        });
    });
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {

        var innerMeesages = new List<string>();
        var inner = ex;

        while (inner != null)
        {
            innerMeesages.Add(inner.Message);
            inner = inner.InnerException;
        }

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Error = "An unexpected error occurred.",
            ExceptionMessages = innerMeesages
        }));
    }
});

app.UseHttpsRedirection();

app.UseMiddleware<ResponseHeadersMiddleware>(); //security headers

app.UseRouting();

// CORS must run after routing so endpoint routing is available
app.UseCors("AllowEnpoints");

// Rate limiting (per-IP) - runs before authentication/authorization
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
