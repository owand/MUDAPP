using MUDAPP.Resources;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MUDAPP.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public Models.Settings.Settings settingsViewModel;

        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = settingsViewModel = new Models.Settings.Settings();

            try
            {
                PickerLanguages.SelectedIndex = settingsViewModel.LangCollection.IndexOf(settingsViewModel?.LangCollection.Where(X => X.LANGNAME == App.AppLanguage).FirstOrDefault());
                PickerThemes.SelectedIndex = settingsViewModel.ThemesCollection.IndexOf(settingsViewModel?.ThemesCollection.Where(X => X.THEMENAME == App.AppTheme).FirstOrDefault());
            }
            catch (Exception)
            {
                PickerLanguages.SelectedIndex = settingsViewModel.LangCollection.IndexOf(settingsViewModel?.LangCollection.Where(X => X.LANGNAME == "en").FirstOrDefault());
                PickerThemes.SelectedIndex = settingsViewModel.ThemesCollection.IndexOf(settingsViewModel?.ThemesCollection.Where(X => X.THEMENAME == "myOSTheme").FirstOrDefault());
            }

            PickerLanguages.SelectedIndexChanged += OnLanguagesChanged;
            PickerThemes.SelectedIndexChanged += OnThemesChanged;
            LayoutChanged += OnSizeChanged; // Определяем обработчик события, которое происходит, когда изменяется ширина или высота.
            IsBusy = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(0);

                    labAppName.Text = AppInfo.Name; // Application Name
                    switch (Xamarin.Forms.Device.RuntimePlatform)
                    {
                        case Xamarin.Forms.Device.iOS:
                        case Xamarin.Forms.Device.Android:
                            labAppVersion.Text = $"{AppInfo.VersionString}." + $"{AppInfo.BuildString}"; // Application Version (1.0.0)
                            break;

                        case Xamarin.Forms.Device.UWP:
                            labAppVersion.Text = $"{AppInfo.VersionString}"; // Application Version (1.0.0)
                            break;

                        default:
                            break;
                    }

                    if (App.AdStateCurrent == true)
                    {
                        slAdblock.IsVisible = false;
                    }
                    else
                    {
                        switch (Connectivity.NetworkAccess)
                        {
                            case NetworkAccess.Internet:
                            case NetworkAccess.ConstrainedInternet:
                                break;

                            default:
                                return;
                        }

                        CheckPurchased();
                    }
                });
            }
            catch (Exception)
            {
                // Что-то пошло не так
                return;
            }
        }

        // Происходит, когда ширина или высота свойств измените значение на этот элемент.
        private void OnSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (Shell.Current.Width > 1000)
                {
                    About.SetValue(Grid.ColumnProperty, 2);
                    About.SetValue(Grid.RowProperty, 0);
                    SettingsContent.ColumnDefinitions[2].Width = 500;
                }
                else
                {
                    About.SetValue(Grid.ColumnProperty, 0);
                    About.SetValue(Grid.RowProperty, 1);
                    SettingsContent.ColumnDefinitions[2].Width = 0;
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnLanguagesChanged(object sender, EventArgs e)
        {
            try
            {
                Xamarin.Essentials.Preferences.Set("currentLanguage", settingsViewModel.LangCollection[PickerLanguages.SelectedIndex].LANGNAME);
                AppResource.Culture = new System.Globalization.CultureInfo(App.AppLanguage);

                //((App)Application.Current).MainPage = new MainPage(); // Refresh App
                ((App)Application.Current).MainPage = new AppShell(); // Refresh App
            }
            catch (Exception)
            {
                return;
            }
        }

        private void OnThemesChanged(object sender, EventArgs e)
        {
            try
            {
                Xamarin.Essentials.Preferences.Set("currentTheme", settingsViewModel.ThemesCollection[PickerThemes.SelectedIndex].THEMENAME);

                switch (settingsViewModel.ThemesCollection[PickerThemes.SelectedIndex].THEMENAME)
                {
                    case "myDarkTheme":
                        App.Current.UserAppTheme = OSAppTheme.Dark;
                        break;

                    case "myLightTheme":
                        App.Current.UserAppTheme = OSAppTheme.Light;
                        break;

                    case "myOSTheme":
                        App.Current.UserAppTheme = OSAppTheme.Unspecified;
                        break;

                    default:
                        App.Current.UserAppTheme = OSAppTheme.Unspecified;
                        break;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        // hardware back button
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            try
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await DisplayAlert(AppResource.messageTitleExit, AppResource.messageExit, AppResource.messageOk, AppResource.messageСancel))
                    {
                        if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                        {
                            DependencyService.Get<Services.ICloseApplication>().CloseApp();
                        }
                    }
                });
            }
            catch { return false; }
            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return true;
        }

        //------------------Tapped----------------------
        #region Tapped events

        private void OnTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () => { await Shell.Current.GoToAsync(settingsViewModel._SelectedItem.TargetName); });
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

        private void Tapped_privacyPolicy(object sender, EventArgs e)
        {
            Launcher.OpenAsync(new Uri("https://sites.google.com/view/owand/privacy"));
        }

        private void Tapped_siteProject(object sender, EventArgs e)
        {
            Launcher.OpenAsync(new Uri("https://sites.google.com/view/owand/MUDCatalog"));
        }

        private void Tapped_mailAuthor(object sender, EventArgs e)
        {
            Launcher.OpenAsync(new Uri("mailto:plowand@outlook.com"));
        }

        private void OpenReviewStore(object sender, EventArgs e)
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.Android:
                    OpenGooglePlay();
                    break;
                case Xamarin.Forms.Device.UWP:
                    OpenMicrosoftStore();
                    break;
                default:
                    break;
            }
        }

        private void OpenStore(object sender, EventArgs e)
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.Android:
                    OpenMicrosoftStore();
                    break;
                case Xamarin.Forms.Device.UWP:
                    OpenGooglePlay();
                    break;
                default:
                    break;
            }
        }

        private void OpenMicrosoftStore()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.Android:
                    Launcher.OpenAsync(new Uri("https://www.microsoft.com/store/apps/9NNR8X05NNR9"));
                    break;
                case Xamarin.Forms.Device.UWP:
                    Launcher.OpenAsync(new Uri("ms-windows-store://pdp/?productid=9NNR8X05NNR9"));
                    break;
                default:
                    break;
            }
        }

        private void OpenGooglePlay()
        {
            Launcher.OpenAsync(new Uri("https://play.google.com/store/apps/details?id=com.plowand.mudapp"));
        }

        #endregion

        //------------------Purchases----------------------
        #region Purchases events

        private async void AdblockPurchase(object sender, EventArgs e)
        {
            if (!Plugin.InAppBilling.CrossInAppBilling.IsSupported)
            {
                return;
            }

            IInAppBilling billing = Plugin.InAppBilling.CrossInAppBilling.Current;
            try
            {
                // For Android
                bool connected = await billing.ConnectAsync(Plugin.InAppBilling.Abstractions.ItemType.InAppPurchase);
                if (!connected)
                {
                    await Application.Current.MainPage.DisplayAlert(AppResource.messageError, AppResource.NoConnected, AppResource.messageOk);
                    return;
                }

                InAppBillingPurchase purchase = await billing.PurchaseAsync("adblock", Plugin.InAppBilling.Abstractions.ItemType.InAppPurchase, "apppayload");
                if (purchase == null) // Покупка неудачна
                {
                    return;
                }
                else if (purchase.State == Plugin.InAppBilling.Abstractions.PurchaseState.Purchased) // Покупка успешна
                {
                    App.AdStateCurrent = true;
                    slAdblock.IsVisible = false;
                }
            }
            catch (Plugin.InAppBilling.Abstractions.InAppBillingPurchaseException ex)
            {
                // Что-то пошло не так
                await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                return;
            }
            catch (Exception ex)
            {
                // Что-то пошло не так
                await DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                return;
            }
            finally
            {
                await billing.DisconnectAsync();
                billing.Dispose();
            }
        }

        // Check Purchases
        private async void CheckPurchased()
        {
            IInAppBilling billing = CrossInAppBilling.Current;
            try
            {
                if (billing == null)
                {
                    return;
                }

                bool connected = await CrossInAppBilling.Current.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                {
                    return;
                }

                //check purchases
                IEnumerable<InAppBillingPurchase> purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                if (purchases == null)
                {
                    return;
                }

                InAppBillingPurchase ADBlock = purchases.FirstOrDefault(p => p.ProductId == "adblock");
                if ((ADBlock == null) || (ADBlock.State == PurchaseState.Refunded) || (ADBlock.State == PurchaseState.Canceled))   // Покупка неудачна
                {
                    App.AdStateCurrent = false;
                }
                else if ((ADBlock.State == PurchaseState.Purchased) || (ADBlock.State == PurchaseState.Purchasing))   // Покупка успешна
                {
                    App.AdStateCurrent = true;
                }

                if (App.AdStateCurrent == true)
                {
                    slAdblock.IsVisible = false;
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                // Что-то пошло не так
                await Application.Current.MainPage.DisplayAlert(AppResource.messageError, purchaseEx.Message, AppResource.messageOk);
                return;
            }
            catch (Exception)
            {
                // Что-то пошло не так
                return;
            }
            finally
            {
                await billing.DisconnectAsync();
                billing.Dispose();
            }
        }

        #endregion
    }
}