using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;

namespace ConsumirApiPelicula.Servicios.IServicios
{
	public interface ISliderService
	{
		Task<ICollection<SliderDTO>> GetSliders();
		Task<RespuestaApi<SliderDTO>> CreateSlider(SliderDTO sliderDTO);
		Task<SliderDTO> GetSliderById(int id);
		Task<RespuestaApi<SliderDTO>> DeleteSlider(int sliderId);
		Task<RespuestaApi<SliderDTO>> UpdateSlider(UpdateSliderDTO sliderDTO);
	}
}
