using ApiPeliculas.Models;
using System.ComponentModel.DataAnnotations;

namespace ConsumirApiPelicula.Models.DTO
{
	public class CrearPeliculaDTO : PeliculaDTO
	{

		public List<int> CategoriasId { get; set; }
	}
}
