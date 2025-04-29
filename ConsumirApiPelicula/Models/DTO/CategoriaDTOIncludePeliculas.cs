using ConsumirApiPelicula.Models.DTO;

namespace ApiPeliculas.Models.DTO
{
	public class CategoriaDTOIncludePeliculas : CategoriaDTO
	{
		public ICollection<PeliculaDTO> Peliculas { get; set; }
	}
}
