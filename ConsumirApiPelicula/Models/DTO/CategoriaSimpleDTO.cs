using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class CategoriaSimpleDTO
	{
			[Key]
			public int Id { get; set; }
			[Required]
			public string Nombre { get; set; }
			public DateTime FechaCreacion { get; set; }
		
	}
}
