using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Controllers {
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase {
		protected readonly IConfiguration _configuration;

		public AuthController(IConfiguration configuration) {
			this._configuration = configuration;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> LoginAsync([FromBody] SignInAttempt login) {
			var token = await AuthenticateAsync(login?.Username, login?.Password);

			if (string.IsNullOrWhiteSpace(token))
				return Unauthorized();

			return Ok(new AuthenticationToken() { Token =  token});
		}

		private async Task<string> AuthenticateAsync(string username, string password) {
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || username != "luiz" && password != "parente")
				return null;

			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration.GetValue<string>("ApiSettings:Authentication:Key")));
			var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
			var tokeOptions = new JwtSecurityToken(
				issuer: this._configuration.GetValue<string>("ApiSettings:Authentication:Origin"),
				audience: this._configuration.GetValue<string>("ApiSettings:Authentication:Origin"),
				claims: new List<Claim>() {
						new Claim(ClaimTypes.Name, username),
						new Claim(ClaimTypes.Role, "Admin")
				},
				expires: DateTime.UtcNow.AddDays(this._configuration.GetValue<int>("ApiSettings:Authentication:DaysToExpireToken")),
				signingCredentials: signinCredentials
			);

			return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
		}

		public class SignInAttempt {
			public string Username { get; set; }
			public string Password { get; set; }
		}

		private class AuthenticationToken {
			public object Token { get; set; }
		}
	}
}
