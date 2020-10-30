using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CRM.Server.ViewFeatures
{
	public class TemplateRenderer
	{
		private const string IEnumerableOfIFormFileName = "IEnumerable`" + nameof(IFormFile);

		public static IEnumerable<string> GetTypeNames(ModelMetadata modelMetadata, Type fieldType)
		{
			// Not returning type name here for IEnumerable<IFormFile> since we will be returning
			// a more specific name, IEnumerableOfIFormFileName.
			TypeInfo? fieldTypeInfo = fieldType.GetTypeInfo();

			if (typeof(IEnumerable<IFormFile>) != fieldType)
			{
				yield return fieldType.Name;
			}

			if (fieldType == typeof(string))
			{
				// Nothing more to provide
				yield break;
			}

			else if (modelMetadata.IsComplexType != true)
			{
				// IsEnum is false for the Enum class itself
				if (fieldTypeInfo.IsEnum)
				{
					// Same as fieldType.BaseType.Name in this case
					yield return "Enum";
				}

				else if (fieldType == typeof(DateTimeOffset))
				{
					yield return "DateTime";
				}

				yield return "String";
				yield break;
			}

			else if (fieldTypeInfo.IsInterface != true)
			{
				Type? type = fieldType;

				while (true)
				{
					type = type.GetTypeInfo().BaseType;

					if (type is not null && type != typeof(object))
					{
						yield return type.Name;
					}

					else
					{
						break;
					}
				}
			}

			if (typeof(IEnumerable).IsAssignableFrom(fieldType))
			{
				if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(fieldType))
				{
					yield return IEnumerableOfIFormFileName;

					// Specific name has already been returned, now return the generic name.
					if (typeof(IEnumerable<IFormFile>) == fieldType)
					{
						yield return fieldType.Name;
					}
				}

				yield return "Collection";
			}

			else if (typeof(IFormFile) != fieldType && typeof(IFormFile).IsAssignableFrom(fieldType))
			{
				yield return nameof(IFormFile);
			}

			yield return "Object";
		}
	}
}