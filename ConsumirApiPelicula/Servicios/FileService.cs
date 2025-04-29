using Microsoft.AspNetCore.Http;
using PiscinaTropical.Utilidades.Servicios.IServicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiscinaTropical.Utilidades.Servicios
{
	 public class FileService : IFileService
	{

		public FileService()
		{
		}

		public async Task<string> SaveFileAsync(IFormFile file,string path, string folderName)
		{
			string uploadsFolder = Path.Combine(path, @"imagenes\" + folderName);
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}

			var extension = Path.GetExtension(file.FileName).ToLower();
			string uniqueFileName = Guid.NewGuid().ToString();
			string filePath = Path.Combine(uploadsFolder, uniqueFileName + extension);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}

			return @"\imagenes\" + folderName + @"\" + uniqueFileName + extension; 
		}

		public async Task DeleteFileAsync(string path, string fileUrl)
		{
			string filePath = Path.Combine(path, fileUrl.TrimStart('\\'));
			if (File.Exists(filePath))
			{
				await Task.Run(() => File.Delete(filePath));
			}
		}
	}
}
