using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace WebAPI {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddAuthentication(opt => {
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(opt => {
				opt.TokenValidationParameters = new TokenValidationParameters() {
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = this.Configuration.GetValue<string>("ApiSettings:Authentication:Origin"),
					ValidAudience = this.Configuration.GetValue<string>("ApiSettings:Authentication:Origin"),
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.GetValue<string>("ApiSettings:Authentication:Key")))
				};
			});

			services.AddCors(opt => {
				opt.AddPolicy("AnyOrigin", builder => {
					builder.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod();
				});
			});

			services.AddResponseCaching();
			services.AddControllers();
			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
			}

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseCors("AnyOrigin");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseResponseCaching();

			// Caching setup.
			//app.Use(async (context, next) => {
			//	context.Response.GetTypedHeaders().CacheControl =
			//		new Microsoft.Net.Http.Headers.CacheControlHeaderValue() {
			//			Public = true,
			//			MaxAge = TimeSpan.FromSeconds(60)
			//		};

			//	context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
			//		new string[] { "Accept-Encoding" };
			//});

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
