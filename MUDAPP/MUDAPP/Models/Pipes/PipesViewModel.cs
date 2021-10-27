using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MUDAPP.Models.Pipes
{
    public class PipesViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();

        public List<PipesTypeModel> TypePipesList { get; set; }  // Collection for filter
        public ObservableCollection<PipesModel> Collection { get; set; }
        public ObservableCollection<PipesModel> PipesODList { get; set; }  // Collection for filter

        public PipesViewModel(string FilterType, string FilterPipesOD)
        {
            if (string.IsNullOrEmpty(FilterType))
            {
                FilterType = "%";
            }

            if (string.IsNullOrEmpty(FilterPipesOD))
            {
                FilterPipesOD = "%";
            }

            TypePipesList = database.Table<PipesTypeModel>().OrderBy(a => a.TYPENAME).ToList();

            PipesODList = new ObservableCollection<PipesModel>(database.Query<PipesModel>($"SELECT " +
                $"tbPipes.PipesOD, " +
                $"tbPipes.PipesODinch " +
                $"FROM tbPipes " +
                $"WHERE " +
                $"tbPipes.PipesTypeID LIKE '{FilterType}' " +
                $"GROUP BY tbPipes.PipesOD ORDER BY tbPipes.PipesOD ASC;"));

            Collection = new ObservableCollection<PipesModel>(database.Query<PipesModel>($"SELECT " +
                $"tbPipes.* FROM tbPipes " +
                $"WHERE " +
                $"(tbPipes.PipesTypeID LIKE '{FilterType}' AND " +
                $"printf('%.2f', tbPipes.PipesOD) LIKE '{FilterPipesOD}') " +
                $"ORDER BY tbPipes.PipesOD ASC;"));


        }

    }



    public class PipesFilterViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();

        public List<PipesTypeModel> TypePipesList { get; set; }  // Collection for filter
        public List<PipesModel> PipesODList { get; set; }  // Collection for filter
        public List<PipesModel> PipesTWList { get; set; }  // Collection for filter

        public PipesFilterViewModel(string FilterType, string FilterPipesOD)
        {
            TypePipesList = new List<PipesTypeModel>(database.Table<PipesTypeModel>().OrderBy(a => a.TYPENAME).ToList());

            PipesODList = new List<PipesModel>(database.Table<PipesModel>().Select(a => a).
                        Where(a => string.IsNullOrEmpty(FilterType) || a.TYPEID.ToString().Equals(FilterType)).
                        GroupBy(a => a.PIPESOD).Select(a => a.First()).OrderBy(a => a.PIPESOD).ToList());

            PipesTWList = new List<PipesModel>(database.Table<PipesModel>().Select(a => a).
                        Where(a => a.TYPEID.ToString().Equals(FilterType) && a.PIPESOD.ToString().Equals(FilterPipesOD)).
                        OrderBy(a => a.PIPESWALL).ToList());
        }
    }
}