using System;
using System.Text.Json.Serialization;

namespace CRM.Server.Models
{
	public class UnsplashModel
	{
		public static UnsplashModel? Unsplash { get; set; }

		[JsonPropertyName("id")]
		public string? Id { get; set; }

		[JsonPropertyName("width")]
		public int Width { get; set; }

		[JsonPropertyName("height")]
		public int Height { get; set; }

		[JsonPropertyName("color")]
		public string? Color { get; set; }

		[JsonPropertyName("description")]
		public string? Description { get; set; }

		[JsonPropertyName("alt_description")]
		public string? AltDescription { get; set; }

		[JsonPropertyName("urls")]
		public Urls? Urls { get; set; }

		[JsonPropertyName("user")]
		public User? User { get; set; }

		[JsonPropertyName("location")]
		public Location? Location { get; set; }
	}

	public class Urls
	{
		[JsonPropertyName("raw")]
		public Uri? Raw { get; set; }

		[JsonPropertyName("full")]
		public Uri? Full { get; set; }

		[JsonPropertyName("regular")]
		public Uri? Regular { get; set; }

		[JsonPropertyName("small")]
		public Uri? Small { get; set; }

		[JsonPropertyName("thumb")]
		public Uri? Thumb { get; set; }
	}

	public class User
	{
		[JsonPropertyName("id")]
		public string? Id { get; set; }

		[JsonPropertyName("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonPropertyName("username")]
		public string? Username { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("first_name")]
		public string? FirstName { get; set; }

		[JsonPropertyName("last_name")]
		public string? LastName { get; set; }

		[JsonPropertyName("twitter_username")]
		public string? TwitterUsername { get; set; }

		[JsonPropertyName("portfolio_url")]
		public Uri? PortfolioUrl { get; set; }

		[JsonPropertyName("bio")]
		public object? Bio { get; set; }

		[JsonPropertyName("location")]
		public string? Location { get; set; }

		[JsonPropertyName("instagram_username")]
		public string? InstagramUsername { get; set; }
	}

	public class Position
	{
		[JsonPropertyName("latitude")]
		public double? Latitude { get; set; }

		[JsonPropertyName("longitude")]
		public double? Longitude { get; set; }
	}

	public class Location
	{
		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("city")]
		public string? City { get; set; }

		[JsonPropertyName("country")]
		public string? Country { get; set; }

		[JsonPropertyName("position")]
		public Position? Position { get; set; }
	}
}