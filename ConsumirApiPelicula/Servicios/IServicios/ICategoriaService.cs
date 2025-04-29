using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsumirApiPelicula.Servicios.IServicios
{
	public interface ICategoriaService
	{
		Task<ICollection<CategoriaDTO>> GetCategorias();
		Task<RespuestaApi<CategoriaDTO>> GetCategoriaById(int id);
		Task<RespuestaApi<CategoriaDTO>> CreateCategoria(CrearCategoriaDTO crearCategoriaDTO);
		Task<RespuestaApi<CategoriaDTO>> UpdateCategoria(CategoriaDTO newCategoria);
		Task<RespuestaApi<CategoriaDTO>> DeleteCategoria(int categoriaId);
		Task<IEnumerable<SelectListItem>> GetListPeliculas();
		Task<ICollection<CategoriaDTOIncludePeliculas>> GetCategoriasIncludePeliculas();
		Task<IEnumerable<SelectListItem>> GetListCategoriasIncludePeliculas();
	}
}
