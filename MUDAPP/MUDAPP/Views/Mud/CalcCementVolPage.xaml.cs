using MUDAPP.Models.Bits;
using MUDAPP.Models.Calc;
using MUDAPP.Models.Pipes;
using MUDAPP.Resources;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MUDAPP.Views.Mud
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalcCementVolPage : ContentPage
    {
        private CalcCementViewModel viewModel = null;
        public CalcCementModel calcCementItem = null;
        private BitODViewModel bitODVM = null;
        private PipesTypeViewModel pipesTypeVM = null;
        private PipesFilterViewModel pipesODVM = null;
        private PipesFilterViewModel pipesODprVM = null;
        private PipesFilterViewModel pipesTVM = null;
        private PipesFilterViewModel pipesTprVM = null;

        public double PI; // Число Пи
        public int TypePipe { get; set; } // Переменная для фильтра по диаметру трубы
        public int TypePipePrev { get; set; } // Переменная для фильтра по диаметру трубы
        public string PipeOD { get; set; } // Переменная для фильтра по диаметру трубы
        public string PipePrevOD { get; set; } // Переменная для фильтра по диаметру трубы

        public CalcCementVolPage()
        {
            InitializeComponent();
            Shell.Current.FlyoutIsPresented = false;

            PI = 3.14159265358979323846;

            LayoutChanged += OnSizeChanged; // Определяем обработчик события, которое происходит, когда изменяется ширина или высота.
            Shell.Current.Navigating += Current_Navigating; // Определяем обработчик события Shell.OnNavigating
        }

        // События непосредственно перед тем как страница становится видимой.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(async () =>
            {
                indicator.IsRunning = true;
                IsBusy = true; ;  // Затеняем задний фон и запускаем ProgressRing
                await System.Threading.Tasks.Task.Delay(100);

                if (viewModel == null) // Если не открыт Picer для выбора картинки в Android
                {
                    viewModel = new CalcCementViewModel();
                }
                BindingContext = viewModel;
                calcCementItem = viewModel?.CalcCementItem; // Производим отбор текущей записи (переменная для загрузки картинки)

                if (bitODVM == null) // Если не открыт Picer для выбора картинки в Android
                {
                    bitODVM = new BitODViewModel();
                }
                picDwell.BindingContext = bitODVM;
                picDwell.SelectedIndex = bitODVM.Collection.IndexOf(bitODVM.Collection.Where(X => X.BITOD == calcCementItem.DWELL).FirstOrDefault());

                //type of the casing
                if (pipesTypeVM == null) // Если не открыт Picer для выбора картинки в Android
                {
                    pipesTypeVM = new PipesTypeViewModel();
                }
                picPipeType.BindingContext = picPrevPipeType.BindingContext = pipesTypeVM;
                picPipeType.SelectedIndex = pipesTypeVM.Collection.IndexOf(pipesTypeVM.Collection.Where(X => X.TYPEID == calcCementItem.PIPESTYPEID).FirstOrDefault());
                if (picPipeType.SelectedIndex >= 0)
                {
                    TypePipe = pipesTypeVM.Collection[picPipeType.SelectedIndex].TYPEID;
                }

                //outside diameter of the casing
                pipesODVM = new PipesFilterViewModel(TypePipe.ToString(), string.Empty);
                picODcas.BindingContext = pipesODVM;
                picODcas.SelectedIndex = pipesODVM.PipesODList.IndexOf(pipesODVM.PipesODList.Where(X => X.PIPESOD == calcCementItem.ODCAS).FirstOrDefault());
                if (picODcas.SelectedIndex >= 0)
                {
                    PipeOD = pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD.ToString();
                }
                //thickness of the wall of the cemented casing
                pipesTVM = new PipesFilterViewModel(TypePipe.ToString(), PipeOD);
                picTcas.BindingContext = pipesTVM;
                picTcas.SelectedIndex = pipesTVM.PipesTWList.IndexOf(pipesTVM.PipesTWList.Where(X => X.PIPESWALL == calcCementItem.TCAS).FirstOrDefault());

                //type of the previous casing
                picPrevPipeType.SelectedIndex = pipesTypeVM.Collection.IndexOf(pipesTypeVM.Collection.Where(X => X.TYPEID == calcCementItem.PIPEPREVTYPEID).FirstOrDefault());
                if (picPrevPipeType.SelectedIndex >= 0)
                {
                    TypePipePrev = pipesTypeVM.Collection[picPrevPipeType.SelectedIndex].TYPEID;
                }
                //outer diameter of the previous casing
                pipesODprVM = new PipesFilterViewModel(TypePipePrev.ToString(), string.Empty);
                picODprcas.BindingContext = pipesODprVM;
                picODprcas.SelectedIndex = pipesODprVM.PipesODList.IndexOf(pipesODprVM.PipesODList.Where(X => X.PIPESOD == calcCementItem.ODPRCAS).FirstOrDefault());
                if (picODprcas.SelectedIndex >= 0)
                {
                    PipePrevOD = pipesODprVM.PipesODList[picODprcas.SelectedIndex].PIPESOD.ToString();
                }
                //thickness of the wall of the cemented casing
                pipesTprVM = new PipesFilterViewModel(TypePipePrev.ToString(), PipePrevOD);
                picTprcas.BindingContext = pipesTprVM;
                picTprcas.SelectedIndex = pipesTprVM.PipesTWList.IndexOf(pipesTprVM.PipesTWList.Where(X => X.PIPESWALL == calcCementItem.TPRCAS).FirstOrDefault());
                IsBusy = false;
                indicator.IsRunning = false;
            });
        }

        private void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.CanCancel)
            {
                e.Cancel(); // Позволяет отменить навигацию
                OnBackButtonPressed();
            }
        }

        // Происходит, когда ширина или высота свойств измените значение на этот элемент.
        private void OnSizeChanged(object sender, EventArgs e)
        {
            switch (Device.Idiom)
            {
                case TargetIdiom.Desktop:
                case TargetIdiom.Tablet:
                    if (Shell.Current.Width > 1000)
                    {
                        Formula.SetValue(Grid.ColumnProperty, 2);
                        Formula.SetValue(Grid.RowProperty, 1);
                        FormulaContent.ColumnDefinitions[2].Width = 500;
                    }
                    else
                    {
                        Formula.SetValue(Grid.ColumnProperty, 0);
                        Formula.SetValue(Grid.RowProperty, 0);
                        FormulaContent.ColumnDefinitions[2].Width = 0;
                    }
                    break;

                case TargetIdiom.Phone:
                    break;

                default:
                    break;
            }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                OnResult();
            }
            catch { lbVcs.Text = AppResource.FormulaError; }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OnResult();
            }
            catch { lbVcs.Text = AppResource.FormulaError; }
        }

        private void ChangePipeType(object sender, EventArgs e)
        {
            try
            {
                TypePipe = pipesTypeVM.Collection[picPipeType.SelectedIndex].TYPEID;

                // очищаем диаметры
                PipeOD = null;
                pipesODVM = null;
                picODcas.SelectedIndex = -1;
                picODcas.SelectedItem = null;
                picODcas.Behaviors.Clear();

                // очищаем толщину стенки
                picTcas.SelectedIndex = -1;
                picTcas.SelectedItem = null;
                picTcas.Behaviors.Clear();
                pipesTVM = null;

                pipesODVM = new PipesFilterViewModel(TypePipe.ToString(), string.Empty);
                picODcas.BindingContext = pipesODVM;
            }
            catch (Exception ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        private void ChangePrevPipeType(object sender, EventArgs e)
        {
            try
            {
                TypePipePrev = pipesTypeVM.Collection[picPrevPipeType.SelectedIndex].TYPEID;

                // очищаем диаметры
                PipePrevOD = null;
                pipesODprVM = null;
                picODprcas.SelectedIndex = -1;
                picODprcas.SelectedItem = null;
                picODprcas.Behaviors.Clear();

                // очищаем толщину стенки
                picTprcas.SelectedIndex = -1;
                picTprcas.SelectedItem = null;
                picTprcas.Behaviors.Clear();
                pipesTprVM = null;

                pipesODprVM = new PipesFilterViewModel(TypePipePrev.ToString(), string.Empty);
                picODprcas.BindingContext = pipesODprVM;
            }
            catch (Exception ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        // Фильт по диаметру труб.
        private void ChangePipeOD(object sender, EventArgs e)
        {
            if (picODcas.SelectedIndex >= 0)
            {
                PipeOD = pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD.ToString();

                // очищаем толщину стенки
                picTcas.SelectedIndex = -1;
                picTcas.SelectedItem = null;
                pipesTVM = null;
                picTcas.Behaviors.Clear();

                pipesTVM = new PipesFilterViewModel(TypePipe.ToString(), PipeOD);
                picTcas.BindingContext = pipesTVM;
            }
            try
            {
                OnResult();
            }
            catch { lbVcs.Text = AppResource.FormulaError; }
        }

        // Фильт по диаметру труб.
        private void ChangePipePrevOD(object sender, EventArgs e)
        {
            if (picODprcas.SelectedIndex >= 0)
            {
                PipePrevOD = pipesODprVM.PipesODList[picODprcas.SelectedIndex].PIPESOD.ToString();

                // очищаем толщину стенки
                picTprcas.SelectedIndex = -1;
                picTprcas.SelectedItem = null;
                pipesTprVM = null;
                picTprcas.Behaviors.Clear();

                pipesTprVM = new PipesFilterViewModel(TypePipePrev.ToString(), PipePrevOD);
                picTprcas.BindingContext = pipesTprVM;
            }
            try
            {
                OnResult();
            }
            catch { lbVcs.Text = AppResource.FormulaError; }
        }

        // Сохраняем изменения.
        private void OnSave(object sender, EventArgs e)
        {
            try
            {
                calcCementItem.CCS = decimal.Parse(edCcs.Text); //coefficient of cement slurry
                calcCementItem.CCAV = decimal.Parse(edCcav.Text); //coefficient of cavernosity
                calcCementItem.DWELL = bitODVM.Collection[picDwell.SelectedIndex].BITOD; //borehole diameter

                calcCementItem.PIPESTYPEID = pipesTypeVM.Collection[picPipeType.SelectedIndex].TYPEID; //type of the casing
                calcCementItem.ODCAS = pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD; //outside diameter of the casing
                calcCementItem.TCAS = pipesTVM.PipesODList[picTcas.SelectedIndex].PIPESWALL; //thickness of the wall of the cemented casing

                calcCementItem.LCAS = decimal.Parse(edLcas.Text); //casing length
                calcCementItem.LHCP = decimal.Parse(edLhcp.Text); //HCP

                calcCementItem.PIPEPREVTYPEID = pipesTypeVM.Collection[picPrevPipeType.SelectedIndex].TYPEID; //type of the previous casing
                calcCementItem.ODPRCAS = pipesODprVM.PipesODList[picODprcas.SelectedIndex].PIPESOD; //outer diameter of the previous casing
                calcCementItem.TPRCAS = pipesTprVM.PipesODList[picTprcas.SelectedIndex].PIPESWALL; //thickness of the wall of the previous casing

                calcCementItem.LPRCAS = decimal.Parse(edLprcas.Text); //depth of descent of the previous column along the trunk
                calcCementItem.LPRHCP = decimal.Parse(edLprhcp.Text); //PrevHCP

                // Сохраняем изменения в текущей записи.
                viewModel?.UpdateItem(calcCementItem);

                //BindingContext = viewModel = new CalcViewModel();
            }
            catch (Exception ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            try
            {
                edCcs.Text = 0.ToString("N2");
                edCcav.Text = 0.ToString("N2");
                picDwell.SelectedIndex = 0;

                picODcas.SelectedIndex = 0;
                picTcas.SelectedIndex = 0;
                edLcas.Text = 0.ToString("N2");
                edLhcp.Text = 0.ToString("N2");

                picODprcas.SelectedIndex = 0;
                picTprcas.SelectedIndex = 0;
                edLprcas.Text = 0.ToString("N2");
                edLprhcp.Text = 0.ToString("N2");
            }
            catch (Exception ex)
            {
                // Что-то пошло не так
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                });
                return;
            }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnResult()
        {
            try
            {
                //calcCementItem = this.viewModel?.CalcCement.First(); // Производим отбор текущей записи (переменная для загрузки картинки)

                //coefficient of cement slurry
                double Ccs = !string.IsNullOrWhiteSpace(edCcs.Text) ? Convert.ToDouble(edCcs.Text) : 1.05;

                //coefficient of cavernosity
                double Ccav = !string.IsNullOrWhiteSpace(edCcav.Text) ? Convert.ToDouble(edCcav.Text) : 1.2;

                //borehole diameter
                double Dwell = picDwell.SelectedIndex >= 0 ? Convert.ToDouble(bitODVM.Collection[picDwell.SelectedIndex].BITODNAME) : 0;

                //casing type
                double ODcas = picODcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD) : 0;

                //thickness of the wall of the cemented casing
                double Tcas = picTcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesTVM.PipesTWList[picTcas.SelectedIndex].PIPESWALL) : 0;

                //casing length
                double Lcas = !string.IsNullOrWhiteSpace(edLcas.Text) ? Convert.ToDouble(edLcas.Text) : 0;

                //HCP
                double Lhcp = !string.IsNullOrWhiteSpace(edLhcp.Text) ? Convert.ToDouble(edLhcp.Text) : 0;

                //outer diameter of the previous casing
                double ODprcas = picODprcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesODprVM.PipesODList[picODprcas.SelectedIndex].PIPESOD) : 0;

                //thickness of the wall of the previous casing
                double Tprcas = picTprcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesTprVM.PipesTWList[picTprcas.SelectedIndex].PIPESWALL) : 0;

                //depth of descent of the previous column along the trunk
                double Lprcas = !string.IsNullOrWhiteSpace(edLprcas.Text) ? Convert.ToDouble(edLprcas.Text) : 0;

                //PrevHCP
                double Lprhcp = !string.IsNullOrWhiteSpace(edLprhcp.Text) ? Convert.ToDouble(edLprhcp.Text) : 0;


                double result = 0;
                //result = Math.Pow(Dwell / 1000, 2);

                result = PI / 4 * Ccs * (
                        (Ccav * Math.Pow(Dwell / 1000, 2) - Math.Pow(ODcas / 1000, 2)) * (Lcas - Lprcas) +
                        (Math.Pow((ODprcas / 1000 - Math.Pow(Tprcas / 1000, 2)), 2) - Math.Pow(ODcas / 1000, 2)) * (Lprcas - Lprhcp) +
                        (Math.Pow(Dwell / 1000, 2) - Math.Pow(ODcas / 1000, 2)) * Lprhcp +
                        Math.Pow((ODcas - Tcas * 2) / 1000, 2) * Lhcp);

                lbVcs.Text = result.ToString("N2");
            }
            catch (Exception)
            {
                // Что-то пошло не так
                lbVcs.Text = "Error";
                return;
            }
        }

        // hardware back button
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            try
            {
                Shell.Current.Navigating -= Current_Navigating; // Отписываемся от события Shell.OnNavigating
                Device.BeginInvokeOnMainThread(async () => { await Shell.Current.GoToAsync("..", true); });
            }
            catch { return false; }
            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }
    }
}