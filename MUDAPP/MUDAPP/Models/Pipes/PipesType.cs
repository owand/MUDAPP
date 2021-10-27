using SQLite;

namespace MUDAPP.Models.Pipes
{
    //Таблица групп труб
    [Table("tbPipesType")]
    public class PipesTypeModel : ViewModelBase
    {
        [Column("PipesTypeID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int TYPEID { get; set; }   // Уникальный код номенклатурной группы

        [Column("PipesType"), NotNull, Unique, Indexed]
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
        public byte[] PICTURE   // Илюстрация
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(PICTURE));
            }
        }

        public int PIPESTYPEID => TYPEID;  // Вспомогательное поле для связи с таблицей муфт



        public int typeid;
        public string typename;
        public byte[] picture;

        public PipesTypeModel()
        {
        }
    }
}