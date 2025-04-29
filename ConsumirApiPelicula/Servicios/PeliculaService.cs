using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ConsumirApiPelicula.Servicios
{
	public class PeliculaService : IPeliculaService
	{
		private readonly string _usuario;
		private readonly string _clave;
		private readonly string _baseurl;
		private string _token;
		public PeliculaService(IConfiguration configuration)
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

		public async Task<RespuestaApi<PeliculaDTOIncludeCategorias>> CreatePelicula(CrearPeliculaDTO peliculaDTO)
		{
			RespuestaApi<PeliculaDTOIncludeCategorias> respuestaAPI = new RespuestaApi<PeliculaDTOIncludeCategorias>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var content = new StringContent(JsonConvert.SerializeObject(peliculaDTO), Encoding.UTF8, "application/json");

			var response = await cliente.PostAsync($"api/Pelicula/CreatePelicula", content);


			// Manejo de diferentes códigos de respuesta
			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				PeliculaDTOIncludeCategorias pelicula = JsonConvert.DeserializeObject<PeliculaDTOIncludeCategorias>(json_respuesta);
				respuestaAPI.Objeto = pelicula;
				respuestaAPI.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
				//throw new ApiExcepcion((int)response.StatusCode, errorContent);
			}

			return respuestaAPI;
		}


		public async Task<RespuestaApi<PeliculaDTO>> DeletePelicula(int peliculaId)
		{
			RespuestaApi<PeliculaDTO> respuestaApi = new RespuestaApi<PeliculaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);


			var response = await cliente.DeleteAsync($"api/Pelicula/DeletePelicula/{peliculaId}");


			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				CrearPeliculaDTO pelicula = JsonConvert.DeserializeObject<CrearPeliculaDTO>(json_respuesta);
				respuestaApi.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaApi.Estado = false;
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
				//throw new ApiExcepcion((int)response.StatusCode, errorContent);
			}

			return respuestaApi;
		}

		public async Task<ICollection<PeliculaDTO>> FindPeliculasByName(string nombre)
		{
			ICollection<PeliculaDTO> categoria = null;

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync($"api/Pelicula/Buscar/{nombre}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<ICollection<PeliculaDTO>>(json_respuesta);
				categoria = resultado;
			}
			return categoria;
		}


		public async Task<PeliculaDTO> GetPeliculaById(int id)
		{
			PeliculaDTO categoria = new PeliculaDTO();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync($"api/Pelicula/{id}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<PeliculaDTO>(json_respuesta);
				categoria = resultado;
			}
			return categoria;
		}

		public async Task<ICollection<PeliculaDTO>> GetPeliculas()
		{
			List<PeliculaDTO> lista = new List<PeliculaDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync("api/Pelicula");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<List<PeliculaDTO>>(json_respuesta);
				lista = resultado;
			}

			return lista;
		}

		public async Task<ICollection<PeliculaDTOIncludeCategorias>> GetPeliculasIncludeCategoria()
		{
			List<PeliculaDTOIncludeCategorias> lista = new List<PeliculaDTOIncludeCategorias>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync("api/Pelicula/GetPeliculasIncludeCategoria");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<List<PeliculaDTOIncludeCategorias>>(json_respuesta);
				lista = resultado;
			}
			return lista;
		}


		public async Task<ICollection<PeliculaDTO>> GetPeliculasByCategoriaId(int categoriaId)
		{
			ICollection<PeliculaDTO> categoria = null;

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync($"api/Pelicula/GetPeliculasByCategoriaId/{categoriaId}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<ICollection<PeliculaDTO>>(json_respuesta);
				categoria = resultado;
			}
			return categoria;
		}

	/*	public async Task<RespuestaApi<PeliculaDTOIncludeCategorias>> UpdatePatchPelicula(CrearPeliculaDTO peliculaDTOBefore, CrearPeliculaDTO peliculaDTOAfter)
		{
			RespuestaApi<PeliculaDTOIncludeCategorias> respuestaAPI = new RespuestaApi<PeliculaDTOIncludeCategorias>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			// Generar el JsonPatchDocument con los cambios detectados
			var patchGenerator = new PatchDocumentGenerator();
			var patchDoc = patchGenerator.GeneratePatch(peliculaDTOBefore, peliculaDTOAfter);

			// Serializar el JsonPatchDocument a JSON
			var jsonPatch = JsonConvert.SerializeObject(patchDoc);
			var content = new StringContent(jsonPatch, Encoding.UTF8, "application/json");

			var response = await cliente.PatchAsync($"api/Pelicula/CreatePelicula", content);


			// Manejo de diferentes códigos de respuesta
			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				PeliculaDTOIncludeCategorias pelicula = JsonConvert.DeserializeObject<PeliculaDTOIncludeCategorias>(json_respuesta);
				respuestaAPI.Objeto = pelicula;
				respuestaAPI.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
				//throw new ApiExcepcion((int)response.StatusCode, errorContent);
			}

			return respuestaAPI;
		}*/

		public async Task<RespuestaApi<PeliculaDTOIncludeCategorias>> UpdatePelicula(CrearPeliculaDTO crearPeliculaDTO)
		{
			RespuestaApi<PeliculaDTOIncludeCategorias> respuestaAPI = new RespuestaApi<PeliculaDTOIncludeCategorias>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);


			//var content = new StringContent(jsonPatch, Encoding.UTF8, "application/json");
			var content = new StringContent(JsonConvert.SerializeObject(crearPeliculaDTO), Encoding.UTF8, "application/json");

			var response = await cliente.PutAsync($"api/Pelicula/UpdatePelicula/{crearPeliculaDTO.Id}", content);


			// Manejo de diferentes códigos de respuesta
			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				PeliculaDTOIncludeCategorias pelicula = JsonConvert.DeserializeObject<PeliculaDTOIncludeCategorias>(json_respuesta);
				respuestaAPI.Objeto = pelicula;
				respuestaAPI.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
				//throw new ApiExcepcion((int)response.StatusCode, errorContent);
			}

			return respuestaAPI;
		}
	}
}

