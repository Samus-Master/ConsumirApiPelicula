using ApiPeliculas.Models;
using ApiPeliculas.Models.DTO;
using ConsumirApiPelicula.Excepciones;
using ConsumirApiPelicula.Models.DTO;
using ConsumirApiPelicula.Servicios.IServicios;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ConsumirApiPelicula.Servicios
{
	public class SliderService : ISliderService
	{
		private static string _usuario;
		private static string _clave;
		private static string _baseurl;
		private static string _token;

		public SliderService(IConfiguration configuration)
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


		public async Task<RespuestaApi<SliderDTO>> CreateSlider(SliderDTO sliderDTO)
		{
			RespuestaApi<SliderDTO> respuestaAPI = new RespuestaApi<SliderDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var content = new StringContent(JsonConvert.SerializeObject(sliderDTO), Encoding.UTF8, "application/json");

			var response = await cliente.PostAsync($"api/Slider/Create", content);

			if (response.IsSuccessStatusCode)
			{
				// Leer la URL del header Location
				var location = response.Headers.Location?.ToString();

				if (!string.IsNullOrEmpty(location))
				{
					var getResponse = await cliente.GetAsync(location);
					if (getResponse.IsSuccessStatusCode)
					{
						var json = await getResponse.Content.ReadAsStringAsync();
						var slider = JsonConvert.DeserializeObject<SliderDTO>(json);
						respuestaAPI.Objeto = slider;
						respuestaAPI.Estado = true;
						return respuestaAPI;
					}
				}

				// Si no hay Location o falla el GET, usar el contenido del POST directamente
				var json_respuesta = await response.Content.ReadAsStringAsync();
				SliderDTO sliderPost = JsonConvert.DeserializeObject<SliderDTO>(json_respuesta);
				respuestaAPI.Objeto = sliderPost;
				respuestaAPI.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
			}

			return respuestaAPI;
		}


		public async Task<RespuestaApi<SliderDTO>> DeleteSlider(int sliderId)
		{
			RespuestaApi<SliderDTO> respuestaApi = new RespuestaApi<SliderDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);


			var response = await cliente.DeleteAsync($"api/Slider/DeleteSlider/{sliderId}");


			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				SliderDTO slider = JsonConvert.DeserializeObject<SliderDTO>(json_respuesta);
				respuestaApi.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaApi.Estado = false;
				respuestaApi.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
			}

			return respuestaApi;
		}

		public async Task<SliderDTO> GetSliderById(int id)
		{
			SliderDTO slider = new SliderDTO();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync($"api/Slider/GetSliderById/{id}");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<SliderDTO>(json_respuesta);
				slider = resultado;
			}
			return slider;
		}

		public async Task<ICollection<SliderDTO>> GetSliders()
		{
			List<SliderDTO> sliders = new List<SliderDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var response = await cliente.GetAsync("api/Slider/GetSliders");

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				var resultado = JsonConvert.DeserializeObject<List<SliderDTO>>(json_respuesta);
				sliders = resultado;
			}
			return sliders;
		}

		public async Task<RespuestaApi<SliderDTO>> UpdateSlider(UpdateSliderDTO updateSliderDTO)
		{
			RespuestaApi<SliderDTO> respuestaAPI = new RespuestaApi<SliderDTO>();

			await Autenticar();

			var cliente = new HttpClient();
			cliente.BaseAddress = new Uri(_baseurl);
			cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

			var patchDoc = new JsonPatchDocument<UpdateSliderDTO>();

			// Agregar dinámicamente sólo las propiedades que vienen con valor
			patchDoc.Replace(s => s.Estado, updateSliderDTO.Estado);

			// Si no hay ningún cambio, opcionalmente puedes evitar enviar el PATCH
			if (patchDoc.Operations.Count == 0)
			{
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = new ProblemDetails
				{
					Title = "No se proporcionaron datos para actualizar.",
					Status = StatusCodes.Status400BadRequest
				};
				return respuestaAPI;
			}

			var jsonPatch = JsonConvert.SerializeObject(patchDoc);
			var content = new StringContent(jsonPatch, Encoding.UTF8, "application/json-patch+json");

			var response = await cliente.PatchAsync($"api/Slider/UpdateSlider/{updateSliderDTO.Id}", content);

			if (response.IsSuccessStatusCode)
			{
				var json_respuesta = await response.Content.ReadAsStringAsync();
				SliderDTO slider = JsonConvert.DeserializeObject<SliderDTO>(json_respuesta);
				respuestaAPI.Objeto = slider;
				respuestaAPI.Estado = true;
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				respuestaAPI.Estado = false;
				respuestaAPI.ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(errorContent);
			}

			return respuestaAPI;
		}

	}
}
