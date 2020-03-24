namespace CRM.UWP
{
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
			LoadApplication(new CRM.App());
		}
	}
}