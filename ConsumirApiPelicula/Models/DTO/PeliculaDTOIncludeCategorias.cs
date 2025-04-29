using ConsumirApiPelicula.Models.DTO;

namespace ApiPeliculas.Models.DTO
{
	public class PeliculaDTOIncludeCategorias : PeliculaDTO
	{

		public ICollection<CategoriaDTO> Categorias { get; set; }
	}
}
