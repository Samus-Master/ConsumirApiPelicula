using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;
using ConsumirApiPelicula.Models.ViewModel;
using ConsumirApiPelicula.Servicios;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using PiscinaTropical.Utilidades.Servicios;
using PiscinaTropical.Utilidades.Servicios.IServicios;

namespace ConsumirApiPelicula.Controllers
{
	public class PeliculaController : Controller
	{
		private readonly IPeliculaService _peliculaService;
		private readonly ICategoriaService _categoriaService;
		private readonly IFileService _fileService;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PeliculaController(IPeliculaService peliculaService, ICategoriaService categoriaService, IFileService fileService, IWebHostEnvironment webHostEnvironment)
		{
			_peliculaService = peliculaService;
			_categoriaService = categoriaService;
			_fileService = fileService;
			_webHostEnvironment = webHostEnvironment;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Details(int id)
		{
			return View();
		}


		public async Task<ActionResult> Create()
		{
			var viewModel = new PeliculaViewModel
			{
				PeliculaDTO = new CrearPeliculaDTO(),
				ListaCategorias = await _categoriaService.GetListPeliculas()
			};

			return View(viewModel);
		}


		[HttpPost]
		public async Task<IActionResult> Create([FromForm] IFormFile archivos, [FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			CrearPeliculaDTO peliculaDTO;
			try
			{
				peliculaDTO = JsonConvert.DeserializeObject<CrearPeliculaDTO>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los dats" });
			}

			/*if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}*/

			if (archivos == null || archivos.Length == 0)
			{
				return BadRequest(new { message = "Seleccione una imagen" });
			}

			try
			{

				peliculaDTO.RutaImagen = await _fileService.SaveFileAsync(archivos, _webHostEnvironment.WebRootPath, peliculaDTO.Clasificacion.ToString());
			}
			catch (Exception e)
			{
				return BadRequest(new { message = $"Error al subir la imagen: {e.Message}" });
			}

			peliculaDTO.FechaCreacion = DateTime.Now;
			peliculaDTO.Duracion = 1;

			try
			{
				RespuestaApi<PeliculaDTOIncludeCategorias> respuestaApi = await _peliculaService.CreatePelicula(peliculaDTO);

				if (respuestaApi.Estado)
				{

					return CreatedAtAction(nameof(Create), new { id = peliculaDTO.Id }, respuestaApi.Objeto);
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

		public ActionResult Edit(int id)
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		public ActionResult Delete(int id)
		{
			return View();
		}


		public async Task<IActionResult> GetAll()
		{
			ICollection<PeliculaDTO> listaPeliculas = await _peliculaService.GetPeliculas();

			return Json(new { data = listaPeliculas });
		}

		public async Task<IActionResult> GetPeliculasIncludeCategoria()
		{
			ICollection<PeliculaDTOIncludeCategorias> listaPeliculasIncludeCategoria = await _peliculaService.GetPeliculasIncludeCategoria();

			return Json(new { data = listaPeliculasIncludeCategoria });
		}


		[HttpDelete]
		public async Task<ActionResult> Delete([FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			int peliculaId;
			try
			{
				peliculaId = JsonConvert.DeserializeObject<int>(modelo);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = "Rellene correctamente los datos" });
			}

			try
			{
				PeliculaDTO peliculaDTO = await _peliculaService.GetPeliculaById(peliculaId);
				await _fileService.DeleteFileAsync(_webHostEnvironment.WebRootPath, peliculaDTO.RutaImagen);
			}
			catch (Exception e)
			{
				return BadRequest(new { message = $"${e.Message}" });
			}

			try
			{
				RespuestaApi<PeliculaDTO> respuestaApi = await _peliculaService.DeletePelicula(peliculaId);

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

		[HttpPut]
		public async Task<IActionResult> Update([FromForm] IFormFile archivos, [FromForm] string modelo)
		{
			if (modelo == null)
			{
				return BadRequest(new { message = "No puede enviar un modelo vacio" });
			}
			CrearPeliculaDTO peliculaDTOUpdate;
			try
			{
				peliculaDTOUpdate = JsonConvert.DeserializeObject<CrearPeliculaDTO>(modelo);
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
				if (peliculaDTOUpdate.RutaImagen is not null)
				{
					var peliculaBefore = await _peliculaService.GetPeliculaById(peliculaDTOUpdate.Id);
					if (peliculaBefore.RutaImagen != peliculaDTOUpdate.RutaImagen)
					{
						await _fileService.DeleteFileAsync(_webHostEnvironment.WebRootPath, peliculaBefore.RutaImagen);
						peliculaDTOUpdate.RutaImagen = await _fileService.SaveFileAsync(archivos, _webHostEnvironment.WebRootPath, peliculaDTOUpdate.Clasificacion.ToString());
					}
				}
			}
			catch (Exception e)
			{
				return BadRequest(new { message = $"Error al cambiar la imagen: {e.Message}" });
			}

			//peliculaDTO.FechaCreacion = DateTime.Now;
			//peliculaDTO.Duracion = 1;


			try
			{
				RespuestaApi<PeliculaDTOIncludeCategorias> respuestaApi = await _peliculaService.UpdatePelicula(peliculaDTOUpdate);

				if (respuestaApi.Estado)
				{

					return CreatedAtAction(nameof(Create), new { id = peliculaDTOUpdate.Id }, respuestaApi.Objeto);
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
	}
}
