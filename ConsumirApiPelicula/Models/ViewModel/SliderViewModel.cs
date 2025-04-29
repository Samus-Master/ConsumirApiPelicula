using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsumirApiPelicula.Models.ViewModel
{
	public class SliderViewModel
	{
		public SliderDTO SliderDTO { get; set; }
		public IEnumerable<SelectListItem> ListaCategorias { get; set; }
	}
}
