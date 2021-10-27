using MUDAPP.Models.Calc;
using MUDAPP.Models.Mud;
using MUDAPP.Resources;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MUDAPP.Views.Mud
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalcCementWeitPage : ContentPage
    {
        private readonly CalcCementViewModel viewModel = null;
        public CalcCementModel calcCementItem = null;
        private readonly MudFilterViewModel mudViewModel = null;

        public CalcCementWeitPage()
        {
            InitializeComponent();
            Shell.Current.FlyoutIsPresented = false;
            BindingContext = viewModel = new CalcCementViewModel();

            mudViewModel = new MudFilterViewModel();
            picMudName.BindingContext = mudViewModel;

            calcCementItem = viewModel?.CalcCementItem; // Производим отбор текущей записи (переменная для загрузки картинки)
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

            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    indicator.IsRunning = true;
                    IsBusy = true; ;  // Затеняем задний фон и запускаем ProgressRing
                    await System.Threading.Tasks.Task.Delay(100);

                    picMudName.SelectedIndex = mudViewModel.MudList.IndexOf(mudViewModel?.MudList.Where(X => X.MUDID == calcCementItem.MUDID).FirstOrDefault());
                    IsBusy = false;
                    indicator.IsRunning = false;
                });
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
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                OnResult();
            }
            catch { }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                labDensityDryCement.Text = mudViewModel?.MudList[picMudName.SelectedIndex].DENSITY.ToString("N2");
                OnResult();
            }
            catch { }
        }

        // Событие при изменении текста в соответствующих полях.
        private void OnResult()
        {
            double Dfluid = 1;   // Плотность жидкости затворения, г/см3 / Density of mixing fluid, g / cm3
            double Dslurry = 1;   // Плотность тампонажного раствора, г/см3 / The density of cement slurry, g / cm3
            double Dсement = 1;   // Плотность сухого материала, г/см3 / Density of dry material, g / cm3

            try
            {
                if (!string.IsNullOrWhiteSpace(edDensityFluid.Text))
                {
                    Dfluid = Convert.ToDouble(edDensityFluid.Text);
                }

                if (!string.IsNullOrWhiteSpace(edDensitySlurry.Text))
                {
                    Dslurry = Convert.ToDouble(edDensitySlurry.Text);
                }

                if (!string.IsNullOrWhiteSpace(labDensityDryCement.Text))
                {
                    Dсement = Convert.ToDouble(labDensityDryCement.Text);
                }

                double Cws = Dfluid * (Dсement - Dslurry) / (Dсement * (Dslurry - Dfluid));
                double Gcem = Dslurry / (1 + Cws);
                double Gflu = Cws * Gcem;

                labCWaterSolid.Text = Cws.ToString("N2");
                labGсement.Text = Gcem.ToString("N3");
                labGfluid.Text = Gflu.ToString("N3");
                if (!string.IsNullOrEmpty(edVcs.Text))
                {
                    double Vcs = double.Parse(edVcs.Text);   // Объём цементного раствора, в м3
                    double CLCem = double.Parse(edCLossCement.Text);   // коэффициент потерь сухого цемента (обычно 1,05)
                    double TGcem = Vcs * Gcem * CLCem;
                    labTGcem.Text = TGcem.ToString("N2");

                    double CLWater = double.Parse(edCLossWater.Text);   // Коэффициент потери воды / Water loss coefficient
                    double TGflu = Vcs * Gflu * CLWater / Dfluid;
                    labTGflu.Text = TGflu.ToString("N2");
                }
            }
            catch (Exception)
            {
                // Что-то пошло не так
                return;
            }
        }

        // Сохраняем изменения.
        private void OnSave(object sender, EventArgs e)
        {
            try
            {
                calcCementItem.DENSITYSLURRY = decimal.Parse(edDensitySlurry.Text); // Плотность тампонажного раствора, г/см3 / The density of cement slurry, g / cm3
                calcCementItem.DENSITYFLUID = decimal.Parse(edDensityFluid.Text); // Плотность жидкости затворения, г/см3 / Density of mixing fluid, g / cm3

                calcCementItem.MUDID = mudViewModel.MudList[picMudName.SelectedIndex].MUDID; // Уникальный код реагентов и тампонажных смесей

                calcCementItem.CLOSSCEMENT = decimal.Parse(edCLossCement.Text); // Коэффициент потерь сухого цемента / Dry cement loss ratio
                calcCementItem.CLOSSWATER = decimal.Parse(edCLossWater.Text); // Коэффициент потери воды / Water loss coefficient

                //Сохраняем изменения в текущей записи.
                viewModel?.UpdateItem(calcCementItem);
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
                edDensitySlurry.Text = 0m.ToString("N2");   // Плотность тампонажного раствора, г/см3 / The density of cement slurry, g / cm3
                edDensityFluid.Text = 0m.ToString("N2");   // Плотность жидкости затворения, г/см3 / Density of mixing fluid, g / cm3

                picMudName.SelectedIndex = -1;   // Уникальный код реагентов и тампонажных смесей
                labDensityDryCement.Text = string.Empty;

                edCLossCement.Text = 0m.ToString("N2");   // Коэффициент потерь сухого цемента / Dry cement loss ratio
                edCLossWater.Text = 0m.ToString("N2");   // Коэффициент потери воды / Water loss coefficient

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