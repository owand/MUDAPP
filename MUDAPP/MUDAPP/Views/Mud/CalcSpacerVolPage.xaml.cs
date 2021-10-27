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
    public partial class CalcSpacerVolPage : ContentPage
    {
        private CalcCementViewModel viewModel = null;
        public CalcCementModel calcCementItem = null;
        private readonly PipesTypeViewModel pipesTypeVM = null;
        private PipesFilterViewModel pipesODVM = null;
        private PipesFilterViewModel pipesTVM = null;

        public double PI; // Число Пи
        public int TypePipe { get; set; } // Переменная для фильтра по диаметру трубы
        public string PipeOD { get; set; } // Переменная для фильтра по диаметру трубы

        public CalcSpacerVolPage()
        {
            InitializeComponent();
            Shell.Current.FlyoutIsPresented = false;
            BindingContext = viewModel = new CalcCementViewModel();

            pipesTypeVM = new PipesTypeViewModel();
            picPipeType.BindingContext = pipesTypeVM;

            calcCementItem = viewModel?.CalcCementItem; // Производим отбор текущей записи (переменная для загрузки картинки)

            PI = 3.14159265358979323846;

            LayoutChanged += OnSizeChanged; // Определяем обработчик события, которое происходит, когда изменяется ширина или высота.
            Shell.Current.Navigating += Current_Navigating; // Определяем обработчик события Shell.OnNavigating
        }

        private void Current_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.CanCancel)
            {
                e.Cancel(); // Позволяет отменить навигацию
                OnBackButtonPressed();
            }
        }

        // События непосредственно перед тем как страница становится видимой.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
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

                //type of the casing
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

                IsBusy = false;
                indicator.IsRunning = false;
            });
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
            catch { lbVsp.Text = AppResource.FormulaError; }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OnResult();
            }
            catch { lbVsp.Text = AppResource.FormulaError; }
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
            catch { lbVsp.Text = AppResource.FormulaError; }
        }

        // Сохраняем изменения.
        private void OnSave(object sender, EventArgs e)
        {
            try
            {
                calcCementItem.CCOMPRES = decimal.Parse(edCcompres.Text); //coefficient of compression
                calcCementItem.VPIPELINE = decimal.Parse(edVpipeline.Text); //Pipeline Volume

                calcCementItem.PIPESTYPEID = pipesTypeVM.Collection[picPipeType.SelectedIndex].TYPEID; //type of the casing
                calcCementItem.ODCAS = pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD; //outside diameter of the casing
                calcCementItem.TCAS = pipesTVM.PipesODList[picTcas.SelectedIndex].PIPESWALL; //thickness of the wall of the cemented casing

                calcCementItem.LCAS = decimal.Parse(edLcas.Text); //casing length
                calcCementItem.LHCP = decimal.Parse(edLhcp.Text); //HCP

                // Сохраняем изменения в текущей записи.
                viewModel?.UpdateItem(calcCementItem);

                //BindingContext = viewModel = new CalcViewModel(0);
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
                picODcas.SelectedIndex = 0;
                picTcas.SelectedIndex = 0;
                edLcas.Text = 0.ToString("N2");
                edLhcp.Text = 0.ToString("N2");

                edCcompres.Text = 0.ToString("N2");
                edVpipeline.Text = 0.ToString("N2");
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

                //coefficient of compression
                double Ccompres = !string.IsNullOrWhiteSpace(edCcompres.Text) ? Convert.ToDouble(edCcompres.Text) : 1;

                //Pipeline Volume
                double Vpipeline = !string.IsNullOrWhiteSpace(edVpipeline.Text) ? Convert.ToDouble(edVpipeline.Text) : 0;

                //casing type
                double ODcas = picODcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesODVM.PipesODList[picODcas.SelectedIndex].PIPESOD) : 0;

                //thickness of the wall of the cemented casing
                double Tcas = picTcas.SelectedIndex >= 0 ? Convert.ToDouble(pipesTVM.PipesTWList[picTcas.SelectedIndex].PIPESWALL) : 0;

                //casing length
                double Lcas = !string.IsNullOrWhiteSpace(edLcas.Text) ? Convert.ToDouble(edLcas.Text) : 0;

                //HCP
                double Lhcp = !string.IsNullOrWhiteSpace(edLhcp.Text) ? Convert.ToDouble(edLhcp.Text) : 0;

                double result = 0;

                result = PI / 4 * Ccompres * (Math.Pow((ODcas - Tcas * 2) / 1000, 2) * (Lcas - Lhcp)) + Vpipeline;

                lbVsp.Text = result.ToString("N2");
            }
            catch (Exception)
            {
                // Что-то пошло не так
                lbVsp.Text = "Error";
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