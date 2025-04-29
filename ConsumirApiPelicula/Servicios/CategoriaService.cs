using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ConsumirApiPelicula.Servicios
{
	public class CategoriaService : ICategoriaService
	{
		private static string _usuario;
		private static string _clave;
		private static string _baseurl;
		private static string _token;
		public CategoriaService(IConfiguration configuration)
		{
			_usuario = configuration.GetValue<string>("ApiSetting:usuario");
			_clave = configuration.GetValue<string>("ApiSetting:clave");
			_baseurl = configuration.GetValue<string>("ApiSetting:baseUrl");
		}

		public async Task Autenticar()
		{
			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);

			var credenciales = new UsuarioLoginDto() { NombreUsuario = _usuario, Password = _clave };

			var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");

			var response = await cliente.PostAsync("api/Usuarios/Login", content);
			var json_respuesta = await response.Content.ReadAsStringAsync();

			var resultado = JsonConvert.DeserializeObject<RespuestaAPI>(json_respuesta);

			_token = resultado.Result.Token;
		}

		public async Task<RespuestaApi<CategoriaDTO>> CreateCategoria(CrearCategoriaDTO crearCategoriaDTO)
		{
			RespuestaApi<CategoriaDTO> respuestaApi = new RespuestaApi<CategoriaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var content = new StringContent(JsonConvert.SerializeObject(crearCategoriaDTO), Encoding.UTF8, "application/json");

			var response = await cliente.PostAsync($"api/Categorias/CreateCategoria", content);

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var categoria = JsonConvert.DeserializeObject<CategoriaDTO>(json_respuesta);
				respuestaApi.Objeto = categoria;
				respuestaApi.Estado = true;
			}
			else
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(json_respuesta);
				respuestaApi.Estado = false;
			}

			return respuestaApi;
		}

		public async Task<RespuestaApi<CategoriaDTO>> DeleteCategoria(int categoriaId)
		{
			RespuestaApi<CategoriaDTO> respuestaApi = new RespuestaApi<CategoriaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);


			var response = await cliente.DeleteAsync($"api/Categorias/DeleteCategoria/{categoriaId}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var categoria = JsonConvert.DeserializeObject<CategoriaDTO>(json_respuesta);
				respuestaApi.Estado = true;
			}
			else
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(json_respuesta);
				respuestaApi.Estado = false;
			}

			return respuestaApi;
		}

		public async Task<RespuestaApi<CategoriaDTO>> GetCategoriaById(int id)
		{
			RespuestaApi<CategoriaDTO> respuestaApi = new RespuestaApi<CategoriaDTO>();


			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync($"api/Categorias/{id}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var categoria = JsonConvert.DeserializeObject<CategoriaDTO>(json_respuesta);
				respuestaApi.Objeto = categoria;
				respuestaApi.Estado = true;
			}
			else
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(json_respuesta);
				respuestaApi.Estado = false;
			}

			return respuestaApi;



		}

		public async Task<ICollection<CategoriaDTO>> GetCategorias()
		{
			List<CategoriaDTO> lista = new List<CategoriaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync("api/Categorias");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<List<CategoriaDTO>>(json_respuesta);
				lista = resultado;
			}
			return lista;
		}

		public async Task<ICollection<CategoriaDTOIncludePeliculas>> GetCategoriasIncludePeliculas()
		{
			List<CategoriaDTOIncludePeliculas> lista = new List<CategoriaDTOIncludePeliculas>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync("api/Categorias/GetCategoriasIncludePeliculas");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<List<CategoriaDTOIncludePeliculas>>(json_respuesta);
				lista = resultado;
			}
			return lista;
		}


		public async Task<IEnumerable<SelectListItem>> GetListPeliculas()
		{
			ICollection<CategoriaDTO> categoriaDTOs = await GetCategorias();

			return categoriaDTOs.Select(categoriaDTO => new SelectListItem
			{
				Text = categoriaDTO.Nombre,
				Value = categoriaDTO.Id.ToString()
			});
		}

		public async Task<IEnumerable<SelectListItem>> GetListCategoriasIncludePeliculas()
		{
			ICollection<CategoriaDTOIncludePeliculas> categoriaDTOIncludePeliculas = await GetCategoriasIncludePeliculas();

			// Crear la lista de SelectListItem
			var selectListItems = new List<SelectListItem>(); // Lista de opciones

			foreach (var categoria in categoriaDTOIncludePeliculas)
			{
				if (categoria.Peliculas.Count > 0) // Solo añadimos categorías que tienen películas
				{
					// Creamos el grupo de opciones para la categoría
					var grupo = new SelectListGroup { Name = categoria.Nombre };

					// Añadimos las películas de la categoría como opciones del select
					foreach (var pelicula in categoria.Peliculas)
					{
						var option = new SelectListItem
						{
							Text = pelicula.Nombre,  // Nombre de la película que se mostrará en el select
							Value = pelicula.Id.ToString(),  // ID de la película como valor del select
							Group = grupo  // Asignamos la película al grupo de la categoría correspondiente
						};

						selectListItems.Add(option); // Añadimos la opción a la lista
					}
				}
			}

			return selectListItems;
		}

		public async Task<RespuestaApi<CategoriaDTO>> UpdateCategoria(CategoriaDTO newCategoria)
		{
			RespuestaApi<CategoriaDTO> respuestaApi = new RespuestaApi<CategoriaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var content = new StringContent(JsonConvert.SerializeObject(newCategoria), Encoding.UTF8, "application/json");

			var response = await cliente.PatchAsync($"api/Categorias/UpdatePatchCategoria/{newCategoria.Id}", content);


			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var categoria = JsonConvert.DeserializeObject<CategoriaDTO>(json_respuesta);
				respuestaApi.Estado = true;
			}
			else
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(json_respuesta);
				respuestaApi.Estado = false;
			}

			return respuestaApi;

		}
	}
}
