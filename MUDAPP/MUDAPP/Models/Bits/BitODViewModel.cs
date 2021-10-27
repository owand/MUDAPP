using SQLite;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MUDAPP.Models.Bits
{
    public class BitODViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();
        public ObservableCollection<BitODModel> Collection { get; set; }

        public BitODViewModel()
        {
            Collection = new ObservableCollection<BitODModel>(database.Table<BitODModel>().OrderBy(a => a.BITOD));
        }

    }


    //Таблица диаметров породоразрушающего инструмента
    [Table("tbBitOD")]
    public class BitODModel : ViewModelBase
    {
        [Column("BitODID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int BITODID { get; set; }   // Уникальный код группы

        [Column("BitOD"), NotNull, Unique, Indexed]
        public decimal BITOD   // Название номенклатурной группы
        {
            get => bitod;
            set
            {
                bitod = value;
                OnPropertyChanged(nameof(BITOD));
            }
        }

        [Column("BitODinch"), NotNull]
        public string BITODINCH   // Наружный диаметр труб, дюйм
        {
            get => bitodinch;
            set
            {
                bitodinch = value;
                OnPropertyChanged(nameof(BITODINCH));
            }
        }

        [Column("Description")]
        public string DESCRIPTION
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(DESCRIPTION));
            }
        }

        public string BITODNAME => bitod.ToString("N2"); // Поле Наружный диаметр труб в американском формате

        public int bitodid;
        public decimal bitod;
        public string bitodinch;
        public string description;

        public BitODModel()
        {
        }
    }

}