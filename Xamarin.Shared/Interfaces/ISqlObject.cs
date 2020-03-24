using System.ComponentModel;

namespace CRM.Shared.Interfaces
{
	public interface ISqlObject : INotifyPropertyChanged
	{
		string? Id { get; }
		//Task GetAsync(int id);
		//Task UploadAsync();
		//Task SaveAsync();
		//Task DeleteAsync();
	}
}