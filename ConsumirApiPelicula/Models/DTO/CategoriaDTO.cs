using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class CategoriaDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(60,ErrorMessage = "el maximo de caracteres es {1}")]
		public string Nombre { get; set; }
	}
}
