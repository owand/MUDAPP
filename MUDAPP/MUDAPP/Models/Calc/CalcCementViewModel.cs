using MUDAPP.Models.Mud;
using MUDAPP.Models.Pipes;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MUDAPP.Models.Calc
{
    public class CalcCementViewModel : ViewModelBase
    {
        private readonly SQLiteConnection database = DependencyService.Get<MUDAPP.Services.ISQLite>().DbConnection();
        private static readonly object collisionLock = new object();
        public ObservableCollection<CalcCementModel> CalcCement { get; set; }
        public CalcCementModel CalcCementItem { get; set; }

        public CalcCementViewModel()
        {
            CalcCement = new ObservableCollection<CalcCementModel>(database.Table<CalcCementModel>());
            // If the table is empty, initialize the collection
            if (!database.Table<CalcCementModel>().Any())
            {
                AddFirstItem();
            }

            CalcCementItem = database.Table<CalcCementModel>().First();
        }

        // Создаем новую пустую запись в основной коллекции
        public CalcCementModel AddFirstItem()
        {
            CalcCementModel NewItem = new CalcCementModel
            {
                CCS = 0,
                CCAV = 0,
                DWELL = 0,
                PIPESTYPEID = 1,  // Нужно разобраться со значением по умолчанию в ComboBox-е  !!!!!!!!!!!!!!!!!!!!
                ODCAS = 0,
                TCAS = 0,
                LCAS = 0,
                LHCP = 0,
                PIPEPREVTYPEID = 1,  // Нужно разобраться со значением по умолчанию в ComboBox-е  !!!!!!!!!!!!!!!!!!!!
                ODPRCAS = 0,
                TPRCAS = 0,
                LPRCAS = 0,
                LPRHCP = 0,
                CCOMPRES = 0,
                VPIPELINE = 0,
                DENSITYSLURRY = 0,
                DENSITYFLUID = 0,
                MUDID = 0,
                CLOSSCEMENT = 0,
                CLOSSWATER = 0
            };

            CalcCement.Add(NewItem);
            return NewItem;
        }

        // Сохраняем или создаем и сохраняем новую запись.
        public void UpdateItem(CalcCementModel temp)
        {
            try
            {
                lock (collisionLock)
                {
                    database.Update(temp);
                }
                database.Close();
            }
            catch (SQLiteException)
            {
            }
        }

    }




    //Таблица параметров формул для расчета объемов и массы тампонажных материалов
    [Table("tbCalcCement")]
    public class CalcCementModel : ViewModelBase
    {
        #region

        [Column("CalcCementID"), PrimaryKey, AutoIncrement, Unique, NotNull, Indexed]
        public int ID { get; set; }   // Уникальный код

        [Column("Ccs")]
        public decimal CCS
        {
            get => ccs;
            set
            {
                ccs = value;
                OnPropertyChanged(nameof(CCS));
            }
        }

        [Column("Ccav")]
        public decimal CCAV
        {
            get => ccav;
            set
            {
                ccav = value;
                OnPropertyChanged(nameof(CCAV));
            }
        }

        [Column("Dwell")]
        public decimal DWELL { get; set; }

        [Column("PipesTypeID"), Indexed, ForeignKey(typeof(PipesTypeModel))]     // Specify the foreign key
        public int PIPESTYPEID { get; set; }   // Номенклатурная группа

        [Column("ODcas")]
        public decimal ODCAS { get; set; }

        [Column("Tcas")]
        public decimal TCAS { get; set; }

        [Column("Lcas")]
        public decimal LCAS { get; set; }

        [Column("Lhcp")]
        public decimal LHCP { get; set; }

        [Column("PipePrevTypeID"), Indexed, ForeignKey(typeof(PipesTypeModel))]     // Specify the foreign key
        public int PIPEPREVTYPEID { get; set; }   // Номенклатурная группа

        [Column("ODprcas")]
        public decimal ODPRCAS { get; set; }

        [Column("Tprcas")]
        public decimal TPRCAS { get; set; }

        [Column("Lprcas")]
        public decimal LPRCAS { get; set; }

        [Column("Lprhcp")]
        public decimal LPRHCP { get; set; }

        [Column("Ccompres")]
        public decimal CCOMPRES { get; set; }

        [Column("Vpipeline")]
        public decimal VPIPELINE { get; set; }

        [Column("DensitySlurry")]   // Плотность тампонажного раствора, г/см3 / The density of cement slurry, g / cm3
        public decimal DENSITYSLURRY { get; set; }

        [Column("DensityFluid")]   // Плотность жидкости затворения, г/см3 / Density of mixing fluid, g / cm3
        public decimal DENSITYFLUID { get; set; }

        [Column("MudID"), Indexed, ForeignKey(typeof(MudModel))]     // Specify the foreign key
        public int MUDID { get; set; }   // Номенклатурная группа

        [Column("CLossCement")]   // Коэффициент потерь сухого цемента / Dry cement loss ratio
        public decimal CLOSSCEMENT { get; set; }

        [Column("CLossWater")]   // Коэффициент потери воды / Water loss coefficient
        public decimal CLOSSWATER { get; set; }



        public decimal ccs;
        public decimal ccav;


        #endregion

        public CalcCementModel()
        {
        }
    }

}