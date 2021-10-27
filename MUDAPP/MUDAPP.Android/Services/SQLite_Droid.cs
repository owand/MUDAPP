using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(MUDAPP.Droid.Services.SQLite_Droid))]
namespace MUDAPP.Droid.Services
{
    public class SQLite_Droid : MUDAPP.Services.ISQLite
    {
        public SQLite_Droid()
        {
        }

        public SQLite.SQLiteConnection DbConnection()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), App.dbName);

            return new SQLite.SQLiteConnection(path);
        }

        public void CreateDBAsync()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), App.dbName);

                // копирование файла из папки Assets по пути path
                if (!File.Exists(path) || (File.Exists(path) && (GetCurrentDBVersion() < App.dbVersion)))
                {
                    Stream dbOpenStream = Android.App.Application.Context.Assets.Open(App.dbName);

                    FileStream dbWriteStream = new FileStream(path, System.IO.FileMode.OpenOrCreate);

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
            catch (Exception)
            {
                return;
            }
        }

        public int GetCurrentDBVersion()  // Get Current Data Base Version
        {
            int currentDbVersion;

            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), App.dbName);
                if (File.Exists(path))
                {
                    SQLite.SQLiteConnection database = new SQLite.SQLiteConnection(path);
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