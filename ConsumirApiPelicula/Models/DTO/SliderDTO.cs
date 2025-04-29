using ConsumirApiPelicula.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTO
{
	public class SliderDTO
	{
		public int Id { get; set; }
		[Required]
		public bool Estado { get; set; }
		[Required]
		[Display(Name = "Pelicula")]
		public int PeliculaId { get; set; }
		public PeliculaDTO Pelicula { get; set; }
	}
}
