using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class CrearCategoriaDTO
	{
		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(60, ErrorMessage = "el maximo de caracteres es {1}")]
		public string Nombre { get; set; }
	}
}
