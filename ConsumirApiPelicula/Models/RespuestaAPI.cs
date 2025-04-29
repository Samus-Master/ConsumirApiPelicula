using ApiPeliculas.Models.DTO;
using System.Net;

namespace ApiPeliculas.Models
{
	public class RespuestaAPI
	{
		public RespuestaAPI()
		{
			ErrorMessages = new List<string>();
		}

		public HttpStatusCode StatusCode { get; set; }
		public bool isSuccess { get; set; } = true;
		public List<string> ErrorMessages { get; set; }
		public UsuarioLoginRespuestaDto Result { get; set; }
	}
}
