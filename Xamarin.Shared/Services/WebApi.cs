using CRM.Shared.Exceptions;
using CRM.Shared.Interfaces;

using Pluralize.NET;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.Shared.Services
{
	public class WebApi
	{
		private const string json = "application/json";
		private readonly Uri baseUri = new Uri("http://localhost:5000");
		private readonly HttpClient httpClient = new HttpClient() { MaxResponseContentBufferSize = 1000000 };
		private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			IgnoreNullValues = true,
			PropertyNameCaseInsensitive = true,
			ReferenceHandling = ReferenceHandling.Preserve,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public WebApi()
		{
			httpClient.DefaultRequestHeaders.Add("X-Version", "1.0");
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(json));
		}

		public async Task<List<T>> GetAsync<T>(CancellationToken CancellationToken = default) where T : ISqlObject, new()
		{
			var requestUri = CreateRequestUri(new T().GetType());
			using HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri, CancellationToken).ConfigureAwait(false);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<List<T>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), jsonSerializerOptions) ?? throw new ArgumentNullException();
			}

			else throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		public async Task<T> GetAsync<T>(string id, CancellationToken CancellationToken = default) where T : ISqlObject, new()
		{
			var requestUri = CreateRequestUri(new T().GetType(), id);
			using HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri, CancellationToken).ConfigureAwait(false);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), jsonSerializerOptions) ?? throw new ArgumentNullException();
			}

			else throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		public async Task<T> PostAsync<T>(T Entity, string path, CancellationToken CancellationToken = default) where T : ISqlObject
		{
			var requestUri = CreateRequestUri(Entity.GetType(), path);
			using var httpStringContent = new StringContent(JsonSerializer.Serialize(Entity, jsonSerializerOptions), Encoding.UTF8, json);
			using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, httpStringContent, CancellationToken).ConfigureAwait(false);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), jsonSerializerOptions) ?? throw new ArgumentNullException();
			}

			else throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		public async Task<T> PostAsync<T>(T Entity, CancellationToken CancellationToken = default) where T : ISqlObject
		{
			var requestUri = CreateRequestUri(Entity.GetType());
			using var httpStringContent = new StringContent(JsonSerializer.Serialize(Entity, jsonSerializerOptions), Encoding.UTF8, json);
			using HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, httpStringContent, CancellationToken).ConfigureAwait(false);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), jsonSerializerOptions) ?? throw new ArgumentNullException();
			}

			else throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		public async Task<T> PutAsync<T>(T Entity, CancellationToken CancellationToken = default) where T : ISqlObject
		{
			var requestUri = CreateRequestUri(Entity.GetType(), Entity.Id);
			using var httpStringContent = new StringContent(JsonSerializer.Serialize(Entity, jsonSerializerOptions), Encoding.UTF8, json);
			using HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(requestUri, httpStringContent, CancellationToken).ConfigureAwait(false);

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false), jsonSerializerOptions) ?? throw new ArgumentNullException();
			}

			else throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		public async Task DeleteAsync<T>(T Entity, CancellationToken CancellationToken = default) where T : ISqlObject
		{
			var requestUri = CreateRequestUri(Entity.GetType(), Entity.Id);
			using HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync(requestUri, CancellationToken).ConfigureAwait(false);
			if (!httpResponseMessage.IsSuccessStatusCode) throw new WebApiException(httpResponseMessage.StatusCode.ToString());
		}

		private Uri CreateRequestUri(Type type, string? path = null)
		{
			return path is null
				? new Uri(baseUri, $"/api/{new Pluralizer().Pluralize(type.Name)}")
				: new Uri(baseUri, $"/api/{new Pluralizer().Pluralize(type.Name)}/{path}");
		}
	}
}