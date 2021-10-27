using MUDAPP.Resources;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MUDAPP.Models.Mud
{
    public class MudTypeViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();
        private static readonly object collisionLock = new object();
        public List<MudTypeModel> Collection { get; set; }
        public List<MudTypeMLModel> MLCollection { get; set; }
        public ObservableCollection<MudTypeJoin> JoinCollection { get; set; }

        private MudTypeJoin _SelectedJoinItem;
        private MudTypeJoin _NewItem = new MudTypeJoin { };

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

        public MudTypeViewModel(string SearchCriterion)
        {
            ReadMode = true;

            Collection = database.Table<MudTypeModel>().OrderBy(a => a.TYPENAME).ToList();

            MLCollection = database.Table<MudTypeMLModel>().Where(a => a.LANGUAGE == App.AppLanguage).ToList();

            IEnumerable<MudTypeJoin> joinList =
            from collection in database.Table<MudTypeModel>()
            join mlCollection in database.Table<MudTypeMLModel>().Where(a => a.LANGUAGE == App.AppLanguage) on collection.TYPEID equals mlCollection.TYPEID into joinCollection
            from mlCollection in joinCollection.DefaultIfEmpty(new MudTypeMLModel { })
            select new MudTypeJoin
            {
                TYPEID = collection.TYPEID,
                TYPENAME = collection.TYPENAME,
                PICTURE = collection.PICTURE,
                SUBID = collection.TYPEID,
                DESCRIPTION = mlCollection.DESCRIPTION,
                NOTE = mlCollection.NOTE,
                LANGUAGE = mlCollection.LANGUAGE
            };

            JoinCollection = new ObservableCollection<MudTypeJoin>(joinList.Where(a =>
                a.TYPENAME.ToLowerInvariant().Contains(SearchCriterion) ||
                (!string.IsNullOrEmpty(a.DESCRIPTION) && a.DESCRIPTION.ToLowerInvariant().Contains(SearchCriterion)) ||
                (!string.IsNullOrEmpty(a.NOTE) && a.NOTE.ToLowerInvariant().Contains(SearchCriterion))).OrderBy(a => a.TYPENAME).ToList());

            // If the table is empty, initialize the collection
            if (!database.Table<MudTypeModel>().Any())
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
                    database.Delete<MudTypeModel>(SelectedJoinItem.TYPEID);
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
        public void UpdateItem(MudTypeModel temp)
        {
            try
            {
                lock (collisionLock)
                {
                    if (SelectedJoinItem.TYPEID == 0)
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
        public void UpdateMLItem(MudTypeMLModel newMLItem, MudTypeMLModel selectMLItem)
        {
            try
            {
                lock (collisionLock)
                {
                    if (MLCollection.FirstOrDefault(mlc => mlc.TYPEID == SelectedJoinItem.TYPEID) == null)
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

        public MudTypeJoin SelectedJoinItem
        {
            get => _SelectedJoinItem;
            set
            {
                _SelectedJoinItem = value;
                OnPropertyChanged();
            }
        }

        public MudTypeJoin NewItem
        {
            get => _NewItem;
            set
            {
                _NewItem = value;
                OnPropertyChanged();
            }
        }
    }


    public class MudTypeJoin : ViewModelBase
    {
        // Catalog
        public int TYPEID { get; set; }   // Уникальный код группы

        public string TYPENAME   // Название номенклатурной группы
        {
            get => typename;
            set
            {
                typename = value;
                OnPropertyChanged(nameof(TYPENAME));
            }
        }

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
            get => typeid;
            set
            {
                typeid = value;
                OnPropertyChanged(nameof(TYPEID));
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

        // Catalog
        public int typeid;
        public string typename;
        public byte[] picture;

        // Sub Catalog
        public string description;
        public string note;
        public string language;

        public MudTypeJoin()
        {
        }
    }
}