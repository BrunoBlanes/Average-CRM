using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace CRM.Server.Interfaces
{
	public interface IWritableOptions<out T> : IOptionsSnapshot<T?> where T : class, new()
	{
		/// <summary>
		/// Asynchronously loop through all the properties of <paramref name="obj"/> creating one <see cref="Action"/> for each property to update its value.
		/// </summary>
		/// <remarks>The <see cref="UpdateAsync(Action{T})"/> method is then called with a combined <see cref="Action"/>&lt;<typeparamref name="T"/>&gt; as its parameter.</remarks>
		/// <param name="obj">The object who's properties will be updated.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task UpdateAsync(object obj);

		/// <summary>
		/// Asynchronously execute an <see cref="Action"/>&lt;<typeparamref name="T"/>&gt; to update a property and save the result to the default writable configuration file.
		/// </summary>
		/// <param name="updateAction">The <see cref="Action"/>&lt;<typeparamref name="T"/>&gt; to be executed.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task UpdateAsync(Action<T> updateAction);
	}
}