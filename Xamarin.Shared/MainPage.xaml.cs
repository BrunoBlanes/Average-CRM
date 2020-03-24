using CRM.Shared.Models;

using System.ComponentModel;

using Xamarin.Forms;

namespace CRM
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private async void Button_Clicked(object sender, System.EventArgs e)
		{
			var user = new User(EmailInputView.Text, PasswordInputView.Text, CPFInputView.Text);
			await user.RegisterAsync();
		}
	}
}
