
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SlotGameBackend.Models;
using SlotGameBackend.Services;
using System.Security.Claims;
using System.Text;

namespace SlotGameBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DBConn");

            builder.Services.AddDbContext<slotDataContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            builder.Services.AddControllers();
          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        NameClaimType = ClaimTypes.Email
                    };
                });


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAdminServices,AdminServices>();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IProvablyFairService, provablyFairService>();
            builder.Services.AddScoped<IWalletService, WalletServices>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1",
                });

                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Apply the security definition only to endpoints with [Authorize] attribute
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });




            var app = builder.Build();

          
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<slotDataContext>();
                context.Database.Migrate();
                context.SeedData();
            }

            app.Run();
        }
    }
}
