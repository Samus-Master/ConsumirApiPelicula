using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;
using ConsumirApiPelicula.Servicios;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;

namespace ConsumirApiPelicula.Controllers
{
	public class CategoriaController : Controller
	{
		private readonly ICategoriaService _categoriaSerivce;

		public CategoriaController(ICategoriaService categoriaService)
		{
			_categoriaSerivce = categoriaService;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			return View();
		}

		//api
		public async Task<IActionResult> GetAll()
		{
			ICollection<CategoriaDTO> listaCategoria = await _categoriaSerivce.GetCategorias();

			return Json(new { data = listaCategoria });
		}

		public async Task<IActionResult> GetCategoriasIncludePeliculas()
		{
			ICollection<CategoriaDTOIncludePeliculas> listaCategoria = await _categoriaSerivce.GetCategoriasIncludePeliculas();

			return Json(new { data = listaCategoria });
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}


			CrearCategoriaDTO crearCategoriaDTO;
			try
			{
				crearCategoriaDTO = JsonConvert.DeserializeObject<CrearCategoriaDTO>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = $"Rellene correctamente los datos {e.Message}" });
			}

			try
			{
				CategoriaDTO categoriaDTO = new CategoriaDTO();
				RespuestaApi<CategoriaDTO> respuestaApi = await _categoriaSerivce.CreateCategoria(crearCategoriaDTO);

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

		[HttpPatch]
		public async Task<IActionResult> Update([FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			CategoriaDTO categoriaDTO;
			try
			{
				categoriaDTO = JsonConvert.DeserializeObject<CategoriaDTO>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los dats" });
			}

			try
			{
				RespuestaApi<CategoriaDTO> respuestaApi = await _categoriaSerivce.UpdateCategoria(categoriaDTO);

				if (respuestaApi.Estado)
				{
					return CreatedAtAction(nameof(Update), new { id = categoriaDTO.Id }, categoriaDTO);
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
			int categoriaId;

			try
			{
				categoriaId = JsonConvert.DeserializeObject<int>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los dats" });
			}

			try
			{
				RespuestaApi<CategoriaDTO> respuestaApi = await _categoriaSerivce.DeleteCategoria(categoriaId);

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
	}
}
