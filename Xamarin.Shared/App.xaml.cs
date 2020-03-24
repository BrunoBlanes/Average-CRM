using CRM.Shared.Services;

using Xamarin.Forms;

namespace CRM
{
	public partial class App : Application
	{
		public static WebApi WebApi { get; } = new WebApi();

		public App()
		{
			InitializeComponent();
			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
