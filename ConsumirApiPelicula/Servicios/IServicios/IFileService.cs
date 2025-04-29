using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiscinaTropical.Utilidades.Servicios.IServicios
{
	public interface IFileService
	{
		Task<string> SaveFileAsync(IFormFile file, string path,  string folderName);
		Task DeleteFileAsync(string path, string fileUrl);
	}
}
