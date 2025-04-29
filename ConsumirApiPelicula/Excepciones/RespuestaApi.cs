using Microsoft.AspNetCore.Mvc;

namespace ConsumirApiPelicula.Excepciones
{
	public class RespuestaApi<T> where T : class
	{
		public T Objeto { get; set; }
		public ICollection<T> Objetos { get; set; }
		public bool Estado { get; set; }
		public ProblemDetails ProblemDetails { get; set; }
	}
}
