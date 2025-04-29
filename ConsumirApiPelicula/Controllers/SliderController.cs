using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;
using ConsumirApiPelicula.Models.ViewModel;
using ConsumirApiPelicula.Servicios;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConsumirApiPelicula.Controllers
{
	public class SliderController : Controller
	{
		private readonly ISliderService _sliderService;
		private readonly ICategoriaService _categoriaService;

		public SliderController(ISliderService sliderService, ICategoriaService categoriaService)
        {
			_sliderService = sliderService;
			_categoriaService = categoriaService;
		}
        public async Task<IActionResult> Create()
		{
			SliderViewModel sliderViewModel = new SliderViewModel
			{
				SliderDTO = new SliderDTO(),
				ListaCategorias = await _categoriaService.GetListCategoriasIncludePeliculas()
			};


			return View(sliderViewModel);
		}

		#region API
		public async Task<IActionResult> GetAll()
		{
			ICollection<SliderDTO> listaSlider = await _sliderService.GetSliders();

			return Json(new { data = listaSlider });
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}


			SliderDTO sliderDTO;
			try
			{
				sliderDTO = JsonConvert.DeserializeObject<SliderDTO>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = $"Rellene correctamente los datos {e.Message}" });
			}

			try
			{
				SliderDTO categoriaDTO = new SliderDTO();
				RespuestaApi<SliderDTO> respuestaApi = await _sliderService.CreateSlider(sliderDTO);

				if (respuestaApi.Estado)
				{
					categoriaDTO = respuestaApi.Objeto;
					return CreatedAtAction(nameof(Create), new { id = categoriaDTO.Id }, categoriaDTO);
				}

				return StatusCode((int)respuestaApi.ProblemDetails.Status, new { codigo = respuestaApi.ProblemDetails.Status, titulo = respuestaApi.ProblemDetails.Title, mensaje = respuestaApi.ProblemDetails.Detail });

			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}

		}


		[HttpDelete]
		public async Task<IActionResult> Delete([FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			int sliderId;

			try
			{
				sliderId = JsonConvert.DeserializeObject<int>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los datos" });
			}

			try
			{
				RespuestaApi<SliderDTO> respuestaApi = await _sliderService.DeleteSlider(sliderId);

				if (respuestaApi.Estado)
				{
					return Ok(new RespuestaApi<string> { Estado = true });
				}
				return StatusCode((int)respuestaApi.ProblemDetails.Status, new { codigo = respuestaApi.ProblemDetails.Status, titulo = respuestaApi.ProblemDetails.Title, mensaje = respuestaApi.ProblemDetails.Detail });

			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}


		}

		public async Task<IActionResult> Update([FromForm] IFormFile archivos, [FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			UpdateSliderDTO sliderDTOUpdate;
			try
			{
				sliderDTOUpdate = JsonConvert.DeserializeObject<UpdateSliderDTO>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los datos" });
			}

			/*if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}*/

			/*if (archivos == null || archivos.Length == 0)
			{
				return BadRequest(new { message = "Seleccione una imagen" });
			}*/

			try
			{
				RespuestaApi<SliderDTO> respuestaApi = await _sliderService.UpdateSlider(sliderDTOUpdate);

				if (respuestaApi.Estado)
				{

					return CreatedAtAction(nameof(Create), new { id = sliderDTOUpdate.Id }, respuestaApi.Objeto);
				}

				return StatusCode((int)respuestaApi.ProblemDetails.Status, new { codigo = respuestaApi.ProblemDetails.Status, titulo = respuestaApi.ProblemDetails.Title, mensaje = respuestaApi.ProblemDetails.Detail });

			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
			/*catch (ApiExcepcion apiEx)
			{
				return StatusCode(apiEx.Codigo, new { codigo = apiEx.Codigo, mensaje = apiEx.Message });
			}*/


		}

		#endregion
	}
}
