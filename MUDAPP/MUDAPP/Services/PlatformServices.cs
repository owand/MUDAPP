
namespace MUDAPP.Services
{
    //Работа с базой данных
    public interface ISQLite
    {
        SQLite.SQLiteConnection DbConnection();

        int GetCurrentDBVersion(); // Get current Data Base version

        void CreateDBAsync(); // Create Data Base
    }

    //Close App
    public interface ICloseApplication
    {
        void CloseApp();
    }
}
