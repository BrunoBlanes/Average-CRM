using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace CRM.Server.Interfaces
{
	public interface IWritableOptions<out T> : IOptionsSnapshot<T?> where T : class, new()
	{
		Task UpdateAsync(object obj);
		Task UpdateAsync(Action<T> applyChanges);
	}
}