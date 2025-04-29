using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;

namespace ConsumirApiPelicula.Servicios.IServicios
{
	public interface IPeliculaService
	{
		Task<ICollection<PeliculaDTO>> GetPeliculas();
		Task<PeliculaDTO> GetPeliculaById(int id);
		Task<ICollection<PeliculaDTO>> GetPeliculasByCategoriaId(int categoriaId);
		Task<ICollection<PeliculaDTO>> FindPeliculasByName(string nombre);
		//Task<RespuestaApi<PeliculaDTOIncludeCategorias>> UpdatePelicula(CrearPeliculaDTO pelicula);
		Task<RespuestaApi<PeliculaDTO>> DeletePelicula(int peliculaId);
		Task<RespuestaApi<PeliculaDTOIncludeCategorias>> CreatePelicula(CrearPeliculaDTO peliculaDto);
		Task<ICollection<PeliculaDTOIncludeCategorias>> GetPeliculasIncludeCategoria();
		//Task<RespuestaApi<PeliculaDTOIncludeCategorias>> UpdatePatchPelicula(CrearPeliculaDTO peliculaDTOBefore, CrearPeliculaDTO peliculaDTOAfter);
		Task<RespuestaApi<PeliculaDTOIncludeCategorias>> UpdatePelicula(CrearPeliculaDTO crearPeliculaDTO);
	}
}
