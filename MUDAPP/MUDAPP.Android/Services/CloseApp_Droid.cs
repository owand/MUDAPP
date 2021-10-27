using MUDAPP.Droid.Services;
using MUDAPP.Services;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApp_Droid))]
namespace MUDAPP.Droid.Services
{
    public class CloseApp_Droid : ICloseApplication
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}