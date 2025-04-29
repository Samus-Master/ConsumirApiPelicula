using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class UsuarioRegistroDto
	{
		[Required(ErrorMessage = "El usuario es obbigatorio")]
		public string NombreUsuario { get; set; }
		[Required(ErrorMessage = "El nombre es obbigatorio")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "El password es obbigatorio")]
		public string Password { get; set; }
		public string Role { get; set; }
	}
}
