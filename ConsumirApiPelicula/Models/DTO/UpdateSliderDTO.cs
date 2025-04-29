using System.ComponentModel.DataAnnotations;

namespace ConsumirApiPelicula.Models.DTO
{
	public class UpdateSliderDTO
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public bool Estado { get; set; }
	}
}
