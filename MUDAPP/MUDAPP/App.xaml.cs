using MUDAPP.Resources;
using MUDAPP.Services;
using MUDAPP.Views.Settings;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
//[assembly: ExportFont("MaterialIcons-Regular.ttf", Alias = "MaterialIcons")]
namespace MUDAPP
{
    public partial class App : Application
    {
        // Переменные для базы данных
        public const string dbName = "DBCatalog.db";
        public const int dbVersion = 63;

        // Переменные для подключения приложения к личному account Microsoft, используя Microsoft Graph API
        public static string ClientID = "5b5004bd-2c71-4721-8343-ad94828c30ea";
        public static string[] Scopes = { "Files.ReadWrite.All", "Files.ReadWrite.AppFolder" };
        public static object ParentWindow { get; set; }

        //переменные для изменения локализации приложения
        public static string AppLanguage
        {
            get => Xamarin.Essentials.Preferences.Get("currentLanguage", "ru");
            set => Xamarin.Essentials.Preferences.Set("currentLanguage", value);
        }

        //переменные для применения темы
        public static string AppTheme
        {
            get => Xamarin.Essentials.Preferences.Get("currentTheme", "myOSTheme");
            set => Xamarin.Essentials.Preferences.Set("currentTheme", value);
        }

        //переменные для Purchases State
        public static bool AdStateCurrent
        {
            get => Xamarin.Essentials.Preferences.Get("AdStateCurrent", false);
            set => Xamarin.Essentials.Preferences.Set("AdStateCurrent", value);
        }

        public App()
        {
            Device.SetFlags(new string[] { "MediaElement_Experimental", "Shell_UWP_Experimental", "Visual_Experimental",
                                           "CollectionView_Experimental", "FastRenderers_Experimental", "CarouselView_Experimental",
                                           "IndicatorView_Experimental", "RadioButton_Experimental", "AppTheme_Experimental",
                                           "Markup_Experimental", "Expander_Experimental" });
            InitializeComponent();

            // Create Data Base
            DependencyService.Get<ISQLite>().CreateDBAsync();

            // Языковая культура приложения должна определяться как можно раньше.
            AppResource.Culture = new CultureInfo(AppLanguage);

            // Theme of the application
            switch (AppTheme)
            {
                case "myDarkTheme":
                    App.Current.UserAppTheme = OSAppTheme.Dark;
                    break;

                case "myLightTheme":
                    App.Current.UserAppTheme = OSAppTheme.Light;
                    break;

                case "myOSTheme":
                    App.Current.UserAppTheme = OSAppTheme.Unspecified;
                    break;

                default:
                    App.Current.UserAppTheme = OSAppTheme.Unspecified;
                    break;
            }

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
