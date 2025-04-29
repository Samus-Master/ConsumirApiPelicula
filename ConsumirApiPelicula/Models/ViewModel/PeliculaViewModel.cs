using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ConsumirApiPelicula.Models.ViewModel
{
	public class PeliculaViewModel
	{
		public CrearPeliculaDTO PeliculaDTO { get; set; }
		public IEnumerable<SelectListItem> ListaCategorias { get; set; }
	}
}
