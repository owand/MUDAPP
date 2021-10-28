using MUDAPP.Views.Mud;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MUDAPP.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public Command GoMudCatalogCommand { get; private set; }
        public Command GoMudTypeCommand { get; private set; }
        public Command GoCalcVolCommand { get; private set; }
        public Command GoCalcWeitCommand { get; private set; }
        public Command GoCalcSpacerCommand { get; private set; }
        public Command GoToSettingsCommand { get; private set; }

        public AppShell()
        {
            InitializeComponent();


            Routing.RegisterRoute(nameof(MudCatalogPage), typeof(MudCatalogPage));
            Routing.RegisterRoute(nameof(MudTypePage), typeof(MudTypePage));
            Routing.RegisterRoute(nameof(CalcCementVolPage), typeof(CalcCementVolPage));
            Routing.RegisterRoute(nameof(CalcCementWeitPage), typeof(CalcCementWeitPage));
            Routing.RegisterRoute(nameof(CalcSpacerVolPage), typeof(CalcSpacerVolPage));


            GoMudCatalogCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(MudCatalogPage)); /*Shell.Current.FlyoutIsPresented = false;*/ });
            GoMudTypeCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(MudTypePage)); /*Shell.Current.FlyoutIsPresented = false;*/ });
            GoCalcVolCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(CalcCementVolPage)); /*Shell.Current.FlyoutIsPresented = false;*/ });
            GoCalcWeitCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(CalcCementWeitPage)); /*Shell.Current.FlyoutIsPresented = false;*/ });
            GoCalcSpacerCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(CalcSpacerVolPage)); /*Shell.Current.FlyoutIsPresented = false;*/ });

            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            GoToSettingsCommand = new Command(async () => { await Shell.Current.GoToAsync(nameof(SettingsPage)); Shell.Current.FlyoutIsPresented = false; });

            BindingContext = this;
        }

        //private void OpenSettings(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        Device.BeginInvokeOnMainThread(async () =>
        //        {
        //            //await Navigation.PushAsync(new SettingsPage());
        //            await Shell.Current.GoToAsync("settingspage");
        //            Shell.Current.FlyoutIsPresented = false;
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Что-то пошло не так
        //        Device.BeginInvokeOnMainThread(async () =>
        //        {
        //            await Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
        //        });
        //        return;
        //    }
        //}



    }
}