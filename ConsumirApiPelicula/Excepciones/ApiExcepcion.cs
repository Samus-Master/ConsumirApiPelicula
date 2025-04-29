namespace ConsumirApiPelicula.Excepciones
{
	public class ApiExcepcion : Exception
	{
		public int Codigo { get; }

        public ApiExcepcion(int codigo, string mensaje): base(mensaje)
        {
            Codigo = codigo;
        }
    }
}
