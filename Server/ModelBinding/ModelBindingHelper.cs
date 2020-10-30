using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace CRM.Server.ModelBinding
{
	public class ModelBindingHelper
	{
		/// <summary>
		/// Converts the provided <paramref name="value"/> to a value of <see cref="Type"/> <paramref name="type"/>.
		/// </summary>
		/// <param name="value">The value to convert."/></param>
		/// <param name="type">The <see cref="Type"/> for conversion.</param>
		/// <param name="culture">The <see cref="CultureInfo"/> for conversion.</param>
		/// <returns>
		/// The converted value or <c>null</c> if the value could not be converted.
		/// </returns>
		public static object? ConvertTo(object value, Type type, CultureInfo? culture)
		{
			if (type is null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value is null)
			{
				// For value types, treat null values as though they were the default value for the type.
				return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
			}

			if (type.IsAssignableFrom(value.GetType()))
			{
				return value;
			}

			CultureInfo cultureToUse = culture ?? CultureInfo.InvariantCulture;
			return UnwrapPossibleArrayType(value, type, cultureToUse);
		}

		private static object? UnwrapPossibleArrayType(object? value, Type destinationType, CultureInfo culture)
		{
			// array conversion results in four cases, as below
			var valueAsArray = value as Array;

			if (destinationType.IsArray)
			{
				Type? destinationElementType = destinationType.GetElementType();

				if (valueAsArray is not null && destinationElementType is not null)
				{
					// case 1: both destination + source type are arrays, so convert each element
					var converted = (IList)Array.CreateInstance(destinationElementType, valueAsArray.Length);

					for (int i = 0; i < valueAsArray.Length; i++)
					{
						converted[i] = ConvertSimpleType(valueAsArray.GetValue(i), destinationElementType, culture);
					}

					return converted;
				}

				else if (destinationElementType is not null)
				{
					// case 2: destination type is array but source is single element, so wrap element in array + convert
					object? element = ConvertSimpleType(value, destinationElementType, culture);
					var converted = (IList)Array.CreateInstance(destinationElementType, 1);
					converted[0] = element;
					return converted;
				}
			}

			else if (valueAsArray is not null)
			{
				// case 3: destination type is single element but source is array, so extract first element + convert
				if (valueAsArray.Length > 0)
				{
					value = valueAsArray.GetValue(0);
					return ConvertSimpleType(value, destinationType, culture);
				}
				else
				{
					// case 3(a): source is empty array, so can't perform conversion
					return null;
				}
			}

			// case 4: both destination + source type are single elements, so convert
			return ConvertSimpleType(value, destinationType, culture);
		}

		private static object? ConvertSimpleType(object? value, Type destinationType, CultureInfo culture)
		{
			if (value is null || destinationType.IsAssignableFrom(value.GetType()))
			{
				return value;
			}

			// In case of a Nullable object, we try again with its underlying type.
			destinationType = UnwrapNullableType(destinationType);

			// if this is a user-input value but the user didn't type anything, return no value
			if (value is string valueAsString && string.IsNullOrWhiteSpace(valueAsString))
			{
				return null;
			}

			TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
			bool canConvertFrom = converter.CanConvertFrom(value.GetType());

			if (canConvertFrom != true)
			{
				converter = TypeDescriptor.GetConverter(value.GetType());
			}

			if ((canConvertFrom || converter.CanConvertTo(destinationType)) != true)
			{
				// EnumConverter cannot convert integer, so we verify manually
				return destinationType.GetTypeInfo().IsEnum
					&& (value is int || value is uint || value is long || value is ulong
					|| value is short || value is ushort || value is byte || value is sbyte)
					? Enum.ToObject(destinationType, value)
					: throw new InvalidOperationException(
					FormatValueProviderResult_NoConverterExists(value.GetType(), destinationType));
			}

			try
			{
				return canConvertFrom
					? converter.ConvertFrom(null, culture, value)
					: converter.ConvertTo(null, culture, value, destinationType);
			}

			catch (FormatException)
			{
				throw;
			}

			catch (Exception ex)
			{
				if (ex.InnerException is null)
				{
					throw;
				}

				else
				{
					// TypeConverter throws System.Exception wrapping the FormatException,
					// so we throw the inner exception.
					ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

					// This code is never reached because the previous line will always throw.
					throw;
				}
			}
		}

		private static Type UnwrapNullableType(Type destinationType)
		{
			return Nullable.GetUnderlyingType(destinationType) ?? destinationType;
		}

		/// <summary>
		/// The parameter conversion from type '{0}' to type '{1}' failed because no type converter can convert between these types.
		/// </summary>
		private static string FormatValueProviderResult_NoConverterExists(object p0, object p1)
		{
			return string.Format(CultureInfo.CurrentCulture, "The parameter conversion from type '{0}' to type '{1}' failed because no type converter can convert between these types.", p0, p1);
		}
	}
}