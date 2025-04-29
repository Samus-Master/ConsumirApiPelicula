namespace ConsumirApiPelicula.Servicios.IServicios
{
	public interface IAuthService
	{
		Task<string> GetTokenAsync();
	}

}
