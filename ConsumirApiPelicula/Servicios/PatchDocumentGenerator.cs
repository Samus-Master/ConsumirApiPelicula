/*using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsumirApiPelicula.Servicios
{

	public class PatchDocumentGenerator
	{
		public JsonPatchDocument<T> GeneratePatch<T>(T original, T updated) where T : class
		{
			var patchDoc = new JsonPatchDocument<T>();

			// Obtener todas las propiedades públicas del tipo
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				// Obtener los valores de la propiedad en el objeto original y el objeto actualizado
				var originalValue = property.GetValue(original);
				var updatedValue = property.GetValue(updated);

				// Compara los valores; si son diferentes, agrega una operación 'Replace'
				if (!Equals(originalValue, updatedValue))
				{
					// Construir una expresión lambda para la propiedad
					var propertyExpression = CreatePropertyExpression<T>(property);

					// Usar la expresión lambda en el JsonPatchDocument
					patchDoc.Replace(propertyExpression, updatedValue);
				}
			}

			return patchDoc;
		}


		// Método para crear la expresión lambda de la propiedad a partir del nombre de la propiedad
		private static Expression<Func<T, object>> CreatePropertyExpression<T>(PropertyInfo propertyInfo)
		{
			// Crea un parámetro del tipo del objeto (por ejemplo, el objeto User)
			var parameter = Expression.Parameter(typeof(T), "x");

			// Obtiene la propiedad del parámetro
			var propertyAccess = Expression.Property(parameter, propertyInfo);

			// Convierte el valor a tipo object para que coincida con el tipo esperado en la expresión lambda
			var convert = Expression.Convert(propertyAccess, typeof(object));

			// Crea la expresión lambda (por ejemplo, x => x.FirstName)
			return Expression.Lambda<Func<T, object>>(convert, parameter);
		}
	}


}
*/