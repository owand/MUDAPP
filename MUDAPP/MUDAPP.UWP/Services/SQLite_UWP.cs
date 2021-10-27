using MUDAPP.Services;
using MUDAPP.UWP.Services;
using SQLite;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(SQLite_UWP))]
namespace MUDAPP.UWP.Services
{
    public class SQLite_UWP : ISQLite
    {
        public SQLite_UWP()
        {
        }

        public SQLiteConnection DbConnection()
        {
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, MUDAPP.App.dbName);

            return new SQLiteConnection(path);
        }

        public async void CreateDBAsync()
        {
            try
            {
                string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, MUDAPP.App.dbName);

                // копирование файла из папки Assets по пути path
                if (!File.Exists(path) || (File.Exists(path) && (GetCurrentDBVersion() < MUDAPP.App.dbVersion)))
                {
                    StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync($"{MUDAPP.App.dbName}");
                    Stream dbOpenStream = await databaseFile.OpenStreamForReadAsync();

                    FileStream dbWriteStream = new FileStream(path, FileMode.OpenOrCreate);

                    byte[] buffer = new byte[1024];
                    int b = buffer.Length;
                    int length;
                    while ((length = dbOpenStream.Read(buffer, 0, b)) > 0)
                    {
                        dbWriteStream.Write(buffer, 0, length);
                    }
                    dbWriteStream.Flush();
                    dbWriteStream.Close();
                    dbOpenStream.Close();
                }
            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.ToString(), "Sorry!");
                await dialog.ShowAsync();
            }
        }

        // Get current Data Base Version
        public int GetCurrentDBVersion()
        {
            int currentDbVersion;

            try
            {
                string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, MUDAPP.App.dbName);

                if (path != null)
                {
                    SQLiteConnection database = new SQLiteConnection(path);
                    currentDbVersion = database.ExecuteScalar<int>("pragma user_version");
                    database.Close();
                    database.Dispose();
                }
                else
                {
                    currentDbVersion = 0;
                }
                return currentDbVersion;
            }
            catch (Exception)
            {
                currentDbVersion = 0;
                return currentDbVersion;
            }
        }
    }
}
