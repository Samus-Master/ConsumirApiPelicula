using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Servicios.IServicios;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ConsumirApiPelicula.Servicios
{
	public class AuthService : IAuthService
	{
		private readonly string _usuario;
		private readonly string _clave;
		private readonly string _baseurl;
		private string _token;
		private DateTime _tokenExpiration;
		private readonly HttpClient _httpClient;

		public AuthService(IConfiguration configuration)
		{
			_usuario = configuration.GetValue<string>("ApiSetting:usuario");
			_clave = configuration.GetValue<string>("ApiSetting:clave");
			_baseurl = configuration.GetValue<string>("ApiSetting:baseUrl");
			_httpClient = new HttpClient { BaseAddress = new Uri(_baseurl) };
		}

		public async Task<string> GetTokenAsync()
		{
			if (!string.IsNullOrEmpty(_token) && _tokenExpiration > DateTime.UtcNow)
			{
				return _token;
			}

			var credenciales = new UsuarioLoginDto()
			{
				NombreUsuario = _usuario,
				Password = _clave
			};

			var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("api/Usuarios/Login", content);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Error al autenticarse contra la API.");
			}

			var jsonRespuesta = await response.Content.ReadAsStringAsync();
			var resultado = JsonConvert.DeserializeObject<RespuestaAPI>(jsonRespuesta);

			_token = resultado.Result.Token;

			// Calcular expiración del JWT
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(_token);
			_tokenExpiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(jwt.Claims.First(c => c.Type == "exp").Value)).UtcDateTime;

			return _token;
		}
	}
}
