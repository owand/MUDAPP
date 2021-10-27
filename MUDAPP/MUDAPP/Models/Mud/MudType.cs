using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MUDAPP.Models.Mud
{
    //Таблица групп реагентов и тампонажных смесей
    [Table("tbMudType")]
    public class MudTypeModel : ViewModelBase
    {
        [Column("MudTypeID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int TYPEID { get; set; }   // Уникальный код группы

        [Column("MudType"), NotNull, Unique, Indexed]
        public string TYPENAME   // Название номенклатурной группы
        {
            get => typename.ToUpper();
            set
            {
                typename = value.ToUpper();
                OnPropertyChanged(nameof(TYPENAME));
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


        public int typeid;
        public string typename;
        public byte[] picture;

        public MudTypeModel()
        {
        }
    }


    //Таблица групп реагентов и тампонажных смесей
    [Table("tbMudTypeML")]
    public class MudTypeMLModel : ViewModelBase
    {
        [Column("MudTypeMLID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int MUDTYPEMLID { get; set; }   // Уникальный код

        [Column("MudTypeID"), NotNull, Indexed, ForeignKey(typeof(MudTypeModel))]     // Specify the foreign key
        public int TYPEID   // Уникальный код группы
        {
            get => typeid;
            set
            {
                typeid = value;
                OnPropertyChanged(nameof(TYPEID));
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


        public int mudtypemlid;
        public int typeid;
        public string description;
        public string note;
        public string language;

        public MudTypeMLModel()
        {
        }
    }
}