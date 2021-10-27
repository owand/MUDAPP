using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Globalization;

namespace MUDAPP.Models.Pipes
{
    //Таблица сортамента труб
    [Table("tbPipes")]
    public class PipesModel : ViewModelBase
    {
        #region

        [Column("PipesID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int PIPESID { get; set; }   // Уникальный код строки в таблице параметров труб

        [Column("PipesTypeID"), NotNull, Indexed, ForeignKey(typeof(PipesTypeModel))]     // Specify the foreign key
        public int TYPEID   // Номенклатурная группа
        {
            get => typeId;
            set
            {
                typeId = value;
                OnPropertyChanged(nameof(TYPEID));
            }
        }

        [Column("PipesOD"), NotNull]
        public decimal PIPESOD   // Наружный диаметр труб, мм
        {
            get => pipesod;
            set
            {
                pipesod = value;
                OnPropertyChanged(nameof(PIPESOD));
            }
        }

        [Column("PipesODinch"), NotNull]
        public string PIPESODINCH   // Наружный диаметр труб, дюйм
        {
            get => pipesodinch;
            set
            {
                pipesodinch = value;
                OnPropertyChanged(nameof(PIPESODINCH));
            }
        }

        [Column("PipesWall"), NotNull]
        public decimal PIPESWALL   // Толщина стенки трубы
        {
            get => pipeswall;
            set
            {
                pipeswall = value;
                OnPropertyChanged(nameof(PIPESWALL));
            }
        }

        [Column("PipesIND"), NotNull]
        public decimal PIPESIND   // Внутренний диаметр, мм
        {
            get => pipesind;
            set
            {
                pipesind = value;
                OnPropertyChanged(nameof(PIPESIND));
            }
        }

        [Column("PipesMass"), NotNull]
        public decimal PIPESMASS   // Удельный вес одного метра трубы (без муфт)
        {
            get => pipesmass;
            set
            {
                pipesmass = value;
                OnPropertyChanged(nameof(PIPESMASS));
            }
        }

        [Column("PipesGOST")]
        public string PIPESGOST   // ТУ, ГОСТ
        {
            get => pipesgost;
            set
            {
                pipesgost = value;
                OnPropertyChanged(nameof(PIPESGOST));
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

        // Склеиваем Наружный диаметр труб и Толщину стенки трубы
        public string PIPESNAME => string.Format("{0:F2}", pipesod) + " x " + string.Format("{0:F2}", pipeswall);//return pipesod.ToString("F2", usCulture) + " x " + pipeswall.ToString("F2", usCulture);

        //Переменная для использования в 1-м варианте группировки, который выводит только перый символ группы
        public string PIPESODNAME => string.Format("{0:F2}", pipesod) + "  /  " + pipesodinch;//return pipesod.ToString("F2", usCulture) + "  /  " + pipesodinch;

        public string PIPESODFILTER => pipesod.ToString("N2", usCulture); // Поле для переменной фильтра по диаметру в американском формате

        public string PIPESWALLFILTER => pipeswall.ToString("N2", usCulture); // Поле для переменной фильтра по диаметру в американском формате

        public string PIPESODFORMULA => pipesod.ToString("N2"); // Поле для переменной фильтра по диаметру в американском формате

        public string PIPESWALLFORMULA => pipeswall.ToString("N2"); // Поле для переменной фильтра по диаметру в американском формате

        [ManyToOne]      // Many to one relationship with PipesTypes
        public PipesTypeModel PipesTypes { get; set; }

        #endregion

        public NumberFormatInfo usCulture = new CultureInfo("en", false).NumberFormat;

        #region
        public int pipesid;
        public int typeId;
        public decimal pipesod;
        public string pipesodinch;
        public decimal pipeswall;
        public decimal pipesind;
        public decimal pipesmass;
        public string pipesgost;
        public string note;
        #endregion

        public PipesModel()
        {
            //this.TYPEID = 1;  // Нужно разобраться со значением по умолчанию в ComboBox-е  !!!!!!!!!!!!!!!!!!!!
            //this.PIPESOD = 0;
            //this.PIPESODINCH = String.Empty;
            //this.PIPESWALL = 0;
            //this.PIPESIND = 0;
            //this.PIPESMASS = 0;
            //this.PIPESGOST = String.Empty;
            //this.NOTE = String.Empty;
        }
    }
}