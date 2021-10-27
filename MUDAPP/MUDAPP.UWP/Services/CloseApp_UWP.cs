using MUDAPP.Services;
using Windows.UI.Xaml;

[assembly: Xamarin.Forms.Dependency(typeof(MUDAPP.UWP.Services.CloseApp_UWP))]
namespace MUDAPP.UWP.Services
{
    public class CloseApp_UWP : ICloseApplication
    {
        public void CloseApp()
        {
            Application.Current.Exit();
        }
    }
}