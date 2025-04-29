using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Models;
using ConsumirApiPelicula.Models.ViewModel;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConsumirApiPelicula.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ISliderService _sliderService;
		private readonly IPeliculaService _peliculaService;

		public HomeController(ILogger<HomeController> logger, ISliderService sliderService, IPeliculaService peliculaService)
		{
			_logger = logger;
			_sliderService = sliderService;
			_peliculaService = peliculaService;
		}

		public IActionResult Index()
		{
			return View();
		}

		async public Task<IActionResult> Indexx()
		{
			IEnumerable<SliderDTO> listSlider = await _sliderService.GetSliders();
			HomeViewModel viewModel = new HomeViewModel()
			{
				Sliders = await _sliderService.GetSliders(),
				ListPeliculas = await _peliculaService.GetPeliculas()
			};

			return View(viewModel);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
