using SQLite;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MUDAPP.Models.Pipes
{
    public class PipesTypeViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();
        public ObservableCollection<PipesTypeModel> Collection { get; set; }

        public PipesTypeViewModel()
        {
            Collection = new ObservableCollection<PipesTypeModel>(database.Table<PipesTypeModel>().OrderBy(a => a.TYPENAME).ToList());
        }

    }
}