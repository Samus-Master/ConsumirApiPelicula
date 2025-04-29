using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Models.DTO;

namespace ConsumirApiPelicula.Models.ViewModel
{
	public class HomeViewModel
	{
		public IEnumerable<SliderDTO> Sliders { get; set; }
		public IEnumerable<PeliculaDTO> ListPeliculas { get; set; }
		
	}
}
