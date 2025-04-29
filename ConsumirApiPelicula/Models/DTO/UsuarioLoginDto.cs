using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class UsuarioLoginDto
	{
		[Required(ErrorMessage = "El usuario es obbigatorio")]
		public string NombreUsuario { get; set; }
		[Required(ErrorMessage = "El password es obbigatorio")]
		public string Password { get; set; }
	}
}
