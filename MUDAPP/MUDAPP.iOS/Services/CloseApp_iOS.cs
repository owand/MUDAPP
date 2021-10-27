using MUDAPP.Services;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(MUDAPP.iOS.Services.CloseApp_iOS))]
namespace MUDAPP.iOS.Services
{
    public class CloseApp_iOS : ICloseApplication
    {
        public void CloseApp()
        {
            Thread.CurrentThread.Abort();
        }
    }
}