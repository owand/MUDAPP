using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MUDAPP.Models.Mud
{
    //Таблица каталог реагентов и тампонажных смесей
    [Table("tbMud")]
    public class MudModel : ViewModelBase
    {
        [Column("MudID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int MUDID { get; set; }   // Уникальный код реагентов и тампонажных смесей

        [Column("MudName"), Unique, NotNull, Indexed]
        public string MUDNAME
        {
            get => mudname.ToUpper();
            set
            {
                mudname = value.ToUpper();
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

        [Column("Formula")]
        public string FORMULA
        {
            get => formula;
            set
            {
                formula = value;
                OnPropertyChanged(nameof(FORMULA));
            }
        }

        [Column("Density")]
        public decimal DENSITY
        {
            get => density;
            set
            {
                density = value;
                OnPropertyChanged(nameof(DENSITY));
            }
        }

        [Column("MinTemp")]
        public int MinTEMP
        {
            get => minTemp;
            set
            {
                minTemp = value;
                OnPropertyChanged(nameof(MinTEMP));
            }
        }

        [Column("MaxTemp")]
        public int MaxTEMP
        {
            get => maxTemp;
            set
            {
                maxTemp = value;
                OnPropertyChanged(nameof(MaxTEMP));
            }
        }

        [Column("Picture")]
        public byte[] PICTURE
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(PICTURE));
            }
        }


        #region
        public int mudid;
        public string mudname;
        public byte[] picture;
        public int typeId;
        public string formula;
        public decimal density;
        public int minTemp;
        public int maxTemp;
        #endregion

        public MudModel()
        {
        }
    }


    //Таблица каталог реагентов и тампонажных смесей
    [Table("tbMudML")]
    public class MudMLModel : ViewModelBase
    {
        [Column("MudMLID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int MUDMLID { get; set; }   // Уникальный код

        [Column("MudID"), NotNull, Indexed, ForeignKey(typeof(MudModel))]     // Specify the foreign key
        public int MUDID
        {
            get => mudid;
            set
            {
                mudid = value;
                OnPropertyChanged(nameof(MUDID));
            }
        }

        [Column("GOST")]
        public string GOST
        {
            get => gost;
            set
            {
                gost = value;
                OnPropertyChanged(nameof(GOST));
            }
        }

        [Column("Analog")]
        public string ANALOG   // Аналоги
        {
            get => analog;
            set
            {
                analog = value;
                OnPropertyChanged(nameof(ANALOG));
            }
        }

        [Column("Function")]
        public string FUNCTION   // Назначение / применение
        {
            get => function;
            set
            {
                function = value;
                OnPropertyChanged(nameof(FUNCTION));
            }
        }

        [Column("Description")]
        public string DESCRIPTION   // Описание
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(DESCRIPTION));
            }
        }

        [Column("Note")]
        public string NOTE   // Примечания
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(NOTE));
            }
        }

        [Column("Language"), NotNull, Indexed]
        public string LANGUAGE   // Язык
        {
            get => language;
            set
            {
                language = value;
                OnPropertyChanged(nameof(LANGUAGE));
            }
        }



        public int mudmlid;
        public int mudid;
        public string gost;
        public string analog;
        public string function;
        public string description;
        public string note;
        public string language;

        public MudMLModel()
        {
        }
    }
}