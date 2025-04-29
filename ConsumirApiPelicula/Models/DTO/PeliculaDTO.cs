using ApiPeliculas.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ConsumirApiPelicula.Models.DTO
{
	public class PeliculaDTO
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(60, ErrorMessage = "el maximo de caracteres es {1}")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(60, ErrorMessage = "el maximo de caracteres es {1}")]
		public string RutaImagen { get; set; }
		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(60, ErrorMessage = "el maximo de caracteres es {1}")]
		public string Descripcion { get; set; }
		public int Duracion { get; set; }

		public TipoClasificacion Clasificacion { get; set; }
		public DateTime FechaCreacion { get; set; }
	}
}
