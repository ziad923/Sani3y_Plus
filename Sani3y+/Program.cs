using AspNetCoreRateLimit;
using Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sani3y_.Data;
using Sani3y_.Helpers;
using Sani3y_.Hubs;
using Sani3y_.Middleware;
using Sani3y_.Models;
using Sani3y_.Repositry;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.Text;
namespace Sani3y_
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // For testing
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
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
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            // Get the correct connection string based on the environment
            var connectionString = builder.Configuration.GetConnectionString("constr");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>   // adding roles to the email
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10; // publish for changing htis 
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigninKey"])),
                    ValidateLifetime = true, // changes 
                    ClockSkew = TimeSpan.Zero
                };
            }).AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            builder.Services.AddTransient<EmailService>();
            builder.Services.AddHttpClient<IAiIntegrationService, AiIntegrationService>(client =>
                {
                    client.BaseAddress = new Uri("http://62.72.16.178:8000");
                });
            builder.Services.AddSingleton<AvatarService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            builder.Services.AddScoped<ITokenService, TokenServies>();
            builder.Services.AddScoped<ICraftsmanQueryRepo, CraftsmanQueryRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IUserRatingRepository, UserRatingRepository>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<ICraftsmanRecommendationService, CraftsmanRecommendationService>();
            builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<ICraftsmanWorkRepo, CraftsmanWorkRepo>();
            builder.Services.AddScoped<ICraftsmanOrdersRepo, CraftsmanOrdersRepo>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IProfessionRepo, ProfessionRepository>();
            builder.Services.AddScoped<IContactMessageRepo, ContactMessageRepo>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<IVerificationService, VerificationService>();
            builder.Services.AddScoped<IAdminRatingRepo, AdminRatingRepo>();
            //Add Cors
            // AllowAll : Policy Name 
            // Edit this later 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")   
                             .AllowAnyMethod()   
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });
            builder.Services.AddSignalR();
            builder.Configuration.AddJsonFile("rateLimitSettings.json", optional: false, reloadOnChange: true);
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddInMemoryRateLimiting();

            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowFrontend");
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseIpRateLimiting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");
            app.Run();
        }
    }
}
