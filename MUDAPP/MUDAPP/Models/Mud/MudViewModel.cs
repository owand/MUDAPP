using MUDAPP.Resources;
using MUDAPP.Services;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MUDAPP.Models.Mud
{
    public class MudViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<ISQLite>().DbConnection();
        private static readonly object collisionLock = new object();
        public List<MudTypeModel> TypePickerList { get; set; }
        public List<MudModel> Collection { get; set; }
        public List<MudMLModel> MLCollection { get; set; }
        public ObservableCollection<MudJoin> JoinCollection { get; set; }

        private MudJoin _SelectedJoinItem;
        private MudJoin _NewItem = new MudJoin { };

        public bool readMode;
        public bool ReadMode
        {
            get => readMode;
            set
            {
                readMode = value;
                OnPropertyChanged(nameof(ReadMode));
            }
        }

        public bool detailMode;
        public bool DetailMode
        {
            get => detailMode;
            set
            {
                detailMode = value;
                OnPropertyChanged(nameof(DetailMode));
            }
        }

        public MudViewModel(string FilterCriterion, string SearchCriterion)
        {
            ReadMode = true;

            Collection = database.Table<MudModel>().OrderBy(a => a.MUDNAME).ToList();

            MLCollection = database.Table<MudMLModel>().Where(a => a.LANGUAGE == App.AppLanguage).ToList();

            TypePickerList = new List<MudTypeModel>(database.Table<MudTypeModel>().OrderBy(a => a.TYPENAME));

            IEnumerable<MudJoin> joinList =
            from collection in database.Table<MudModel>()
            join mlCollection in database.Table<MudMLModel>().Where(a => a.LANGUAGE == App.AppLanguage) on collection.MUDID equals mlCollection.MUDID into joinCollection
            from mlCollection in joinCollection.DefaultIfEmpty(new MudMLModel { })
            select new MudJoin
            {
                MUDID = collection.MUDID,
                TYPEID = collection.TYPEID,
                MUDNAME = collection.MUDNAME,
                FORMULA = collection.FORMULA,
                DENSITY = collection.DENSITY,
                MinTEMP = collection.MinTEMP,
                MaxTEMP = collection.MaxTEMP,
                PICTURE = collection.PICTURE,

                SUBID = collection.TYPEID,
                GOST = mlCollection.GOST,
                ANALOG = mlCollection.ANALOG,
                FUNCTION = mlCollection.FUNCTION,
                DESCRIPTION = mlCollection.DESCRIPTION,
                NOTE = mlCollection.NOTE,
                LANGUAGE = mlCollection.LANGUAGE
            };

            JoinCollection = new ObservableCollection<MudJoin>(joinList.Where(a =>
                (string.IsNullOrEmpty(FilterCriterion) || a.TYPEID.ToString().Equals(FilterCriterion)) &&
                (a.MUDNAME.ToLowerInvariant().Contains(SearchCriterion) ||
                (!string.IsNullOrEmpty(a.FORMULA) && a.FORMULA.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.GOST) && a.GOST.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.ANALOG) && a.ANALOG.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.FUNCTION) && a.FUNCTION.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.DESCRIPTION) && a.DESCRIPTION.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.NOTE) && a.NOTE.ToLowerInvariant().Contains(SearchCriterion)))).OrderBy(a => a.MUDNAME).ToList());

            foreach (MudJoin element in JoinCollection)
            {
                //if (element != null)
                database.GetChildren(element);
            }

            // If the table is empty, initialize the collection
            if (!database.Table<MudModel>().Any())
            {
                JoinCollection?.Add(NewItem);
            }
        }

        // Удаляем текущую запись в основной коллекции
        public void DeleteItem()
        {
            try
            {
                lock (collisionLock)
                {
                    database.Delete<MudModel>(SelectedJoinItem.MUDID);
                    JoinCollection.Remove(SelectedJoinItem);
                }
            }
            catch (SQLiteException ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        // Сохраняем или создаем запись в основной коллекции
        public void UpdateItem(MudModel temp)
        {
            try
            {
                lock (collisionLock)
                {
                    if (SelectedJoinItem.MUDID == 0)
                    {
                        database.Insert(temp);
                    }
                    else
                    {
                        database.Update(temp);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        // Сохраняем или создаем запись в подчиненной коллекции
        public void UpdateMLItem(MudMLModel newMLItem, MudMLModel selectMLItem)
        {
            try
            {
                lock (collisionLock)
                {
                    if (MLCollection.FirstOrDefault(mlc => mlc.MUDID == SelectedJoinItem.MUDID) == null)
                    {
                        database.Insert(newMLItem);
                    }
                    else
                    {
                        database.Update(selectMLItem);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        public MudJoin SelectedJoinItem
        {
            get => _SelectedJoinItem;
            set
            {
                _SelectedJoinItem = value;
                OnPropertyChanged();
            }
        }

        public MudJoin NewItem
        {
            get => _NewItem;
            set
            {
                _NewItem = value;
                OnPropertyChanged();
            }
        }
    }


    public class MudFilterViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<ISQLite>().DbConnection();
        public List<MudModel> MudList { get; set; }
        public List<MudTypeModel> TypeList { get; set; }

        public MudFilterViewModel()
        {
            MudList = new List<MudModel>(database.Table<MudModel>().OrderBy(a => a.MUDNAME));
            TypeList = new List<MudTypeModel>(database.Table<MudTypeModel>().OrderBy(a => a.TYPENAME));
        }
    }


    public class MudJoin : ViewModelBase
    {
        // Catalog
        public int MUDID { get; set; }   // Уникальный код реагентов и тампонажных смесей

        public string MUDNAME
        {
            get => mudname;
            set
            {
                mudname = value;
                OnPropertyChanged(nameof(MUDNAME));
            }
        }

        [Column("MudTypeID"), NotNull, Indexed, ForeignKey(typeof(MudTypeModel))]     // Specify the foreign key
        public int TYPEID
        {
            get => typeId;
            set
            {
                typeId = value;
                OnPropertyChanged(nameof(TYPEID));
            }
        }

        public string FORMULA
        {
            get => formula;
            set
            {
                formula = value;
                OnPropertyChanged(nameof(FORMULA));
            }
        }

        public decimal DENSITY
        {
            get => density;
            set
            {
                density = value;
                OnPropertyChanged(nameof(DENSITY));
            }
        }
        public string DENSITYFORMAT => string.Format("{0:N3}", density); // Поле в американском формате

        public int MinTEMP
        {
            get => minTemp;
            set
            {
                minTemp = value;
                OnPropertyChanged(nameof(MinTEMP));
            }
        }
        public string MinTEMPFORMAT => string.Format("{0}", minTemp); // Поле в американском формате

        public int MaxTEMP
        {
            get => maxTemp;
            set
            {
                maxTemp = value;
                OnPropertyChanged(nameof(MaxTEMP));
            }
        }
        public string MaxTEMPFORMAT => string.Format("{0}", maxTemp); // Поле в американском формате

        public byte[] PICTURE
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(PICTURE));
            }
        }

        // Sub Catalog
        public int SUBID   // Уникальный код в подчиненной коллекции
        {
            get => mudid;
            set
            {
                mudid = value;
                OnPropertyChanged(nameof(MUDID));
            }
        }

        public string GOST
        {
            get => gost;
            set
            {
                gost = value;
                OnPropertyChanged(nameof(GOST));
            }
        }

        public string ANALOG   // Аналоги
        {
            get => analog;
            set
            {
                analog = value;
                OnPropertyChanged(nameof(ANALOG));
            }
        }

        public string FUNCTION   // Назначение / применение
        {
            get => function;
            set
            {
                function = value;
                OnPropertyChanged(nameof(FUNCTION));
            }
        }

        public string DESCRIPTION   // Описание
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(DESCRIPTION));
            }
        }

        public string NOTE   // Примечания
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(NOTE));
            }
        }

        public string LANGUAGE   // Язык
        {
            get => language;
            set
            {
                language = value;
                OnPropertyChanged(nameof(LANGUAGE));
            }
        }

        [ManyToOne]      // Many to one relationship with MudTypes
        public MudTypeModel MudTypes { get; set; }

        #region
        // Catalog
        public int mudid;
        public string mudname;
        public int typeId;
        public string formula;
        public decimal density;
        public int minTemp;
        public int maxTemp;
        public byte[] picture;

        // Sub Catalog
        public string gost;
        public string analog;
        public string function;
        public string description;
        public string note;
        public string language;
        #endregion

        public MudJoin()
        {
        }
    }
}