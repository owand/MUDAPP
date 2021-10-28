﻿using MUDAPP.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MUDAPP.Models
{
    public class Settings : ViewModelBase
    {
        public List<LangModel> LangCollection { get; }
        public List<ThemesModel> ThemesCollection { get; }

        public ContentList _SelectedItem;
        public List<ContentListGroup> CollectionGroup { get; set; }

        public int AppLanguage => LangCollection.IndexOf(LangCollection.Where(X => X.LANGNAME == App.AppLanguage).FirstOrDefault());
        public int AppTheme => ThemesCollection.IndexOf(ThemesCollection.Where(X => X.THEMENAME == App.AppTheme).FirstOrDefault());

        public Settings()
        {
            LangCollection = new List<LangModel>()
            {
                new LangModel { LANGDISPLAY = AppResource.LanguageRus, LANGNAME = "ru" },
                new LangModel { LANGDISPLAY = AppResource.LanguageEng, LANGNAME = "en" }
            };

            ThemesCollection = new List<ThemesModel>()
            {
                new ThemesModel { THEMEDISPLAY = AppResource.ThemesDarkName, THEMENAME = "myDarkTheme" },
                new ThemesModel { THEMEDISPLAY = AppResource.ThemesLightName, THEMENAME = "myLightTheme" },
                new ThemesModel { THEMEDISPLAY =  AppResource.ThemesOSName, THEMENAME = "myOSTheme" }
            };

            CollectionGroup = new List<ContentListGroup>
            {
                // реагенты и цементы
                new ContentListGroup(AppResource.TitleMudGroup, new List<ContentList> {
                new ContentList() { Title = AppResource.TitleMudCatalog, TargetType = typeof(Views.Mud.MudCatalogPage), TargetName = nameof(Views.Mud.MudCatalogPage) },
                new ContentList() { Title = AppResource.TitleElementGroup, TargetType = typeof(Views.Mud.MudTypePage), TargetName = nameof(Views.Mud.MudTypePage) },
                new ContentList() { Title = AppResource.TitleCalcCementVol, TargetType = typeof(Views.Mud.CalcCementVolPage), TargetName = nameof(Views.Mud.CalcCementVolPage) },
                new ContentList() { Title = AppResource.TitleCalcCementWeight, TargetType = typeof(Views.Mud.CalcCementWeitPage), TargetName = nameof(Views.Mud.CalcCementWeitPage) },
                new ContentList() { Title = AppResource.TitleCalcSpacerVol, TargetType = typeof(Views.Mud.CalcSpacerVolPage), TargetName = nameof(Views.Mud.CalcSpacerVolPage) } }),

                // расчеты в бурении
                //new ContentListGroup(AppResource.TitleCalcGroup, new List<ContentList> {
                //new ContentList() { Title = AppResource.TitleBodyDrop, TargetType = typeof(Views.Calculator.BodyDropPage) } }),
            };
        }


        public ContentList Selecteditem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged(nameof(Selecteditem));
            }
        }

        public async Task ProVersionPurchase()
        {
            //#if DEBUG
            //            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, "Debug version", AppResource.messageOk);
            //            return;
            //#elif RELEASE 

            switch (Xamarin.Essentials.Connectivity.NetworkAccess)
            {
                case Xamarin.Essentials.NetworkAccess.Internet:
                case Xamarin.Essentials.NetworkAccess.ConstrainedInternet:
                    break;

                default:
                    return;
            }

            if (!Plugin.InAppBilling.CrossInAppBilling.IsSupported)
            {
                return;
            }

            Plugin.InAppBilling.IInAppBilling billing = Plugin.InAppBilling.CrossInAppBilling.Current;
            try
            {
                bool connected = await billing.ConnectAsync();
                if (!connected)
                {
                    await billing.DisconnectAsync();
                    return;
                }

                Plugin.InAppBilling.IInAppBillingVerifyPurchase verify = Xamarin.Forms.DependencyService.Get<Plugin.InAppBilling.IInAppBillingVerifyPurchase>();
                Plugin.InAppBilling.InAppBillingPurchase purchase = await billing?.PurchaseAsync(App.ProductID, Plugin.InAppBilling.ItemType.InAppPurchase, verify);

                //check purchases
                System.Collections.Generic.IEnumerable<Plugin.InAppBilling.InAppBillingPurchase> purchases = await billing.GetPurchasesAsync(Plugin.InAppBilling.ItemType.InAppPurchase);

                //check for null just incase
                if (purchases?.Any(p => p.ProductId == App.ProductID) ?? false)
                {
                    // покупка найдена
                    App.ProState = true;
                }

                await billing.DisconnectAsync();
                return;
            }
            catch (Plugin.InAppBilling.InAppBillingPurchaseException ex) // Что-то пошло не так
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                await billing.DisconnectAsync();
                return;
            }
            catch (System.Exception ex) // Что-то пошло не так
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                await billing.DisconnectAsync();
                return;
            }
            finally
            {
                await billing.DisconnectAsync();
            }

            //#endif
        }

        public static async Task ProVersionCheck()
        {
            //#if DEBUG
            //            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, "Debug version", AppResource.messageOk); // Что-то пошло не так
            //            return;
            //#else

            switch (Xamarin.Essentials.Connectivity.NetworkAccess)
            {
                case Xamarin.Essentials.NetworkAccess.Internet:
                case Xamarin.Essentials.NetworkAccess.ConstrainedInternet:
                    break;

                default:
                    return;
            }

            if (!Plugin.InAppBilling.CrossInAppBilling.IsSupported)
            {
                return;
            }

            Plugin.InAppBilling.IInAppBilling billing = Plugin.InAppBilling.CrossInAppBilling.Current;
            try
            {
                bool connected = await billing.ConnectAsync();

                if (!connected)
                {
                    await billing.DisconnectAsync();
                    return; //Couldn't connect
                }

                //check purchases
                System.Collections.Generic.IEnumerable<Plugin.InAppBilling.InAppBillingPurchase> purchases = await billing.GetPurchasesAsync(Plugin.InAppBilling.ItemType.InAppPurchase);

                //check for null just incase
                if (purchases?.Any(p => p.ProductId == App.ProductID) ?? false)
                {
                    // покупка найдена
                    App.ProState = true;
                }
                await billing.DisconnectAsync();
                return;
            }
            catch (Plugin.InAppBilling.InAppBillingPurchaseException ex) // Что-то пошло не так
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk);
                await billing.DisconnectAsync();
                return;
            }
            catch (System.Exception ex)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResource.messageError, ex.Message, AppResource.messageOk); // Что-то пошло не так
                await billing.DisconnectAsync();
                return;
            }
            finally
            {
                await billing.DisconnectAsync();
            }

            //#endif
        }

    }


    public class ContentList
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
        public ShellNavigationState TargetName { get; set; }

        public ContentList()
        {
        }
    }

    public class ContentListGroup : ObservableCollection<ContentList>
    {
        public string GroupName { get; set; }
        public List<ContentList> SourceList { get; set; }

        public ContentListGroup(string name, List<ContentList> source) : base(source)
        {
            GroupName = name;
            SourceList = source;
        }
    }

    public class LangModel
    {
        public string LANGNAME { get; set; }

        public string LANGDISPLAY { get; set; }

        public LangModel()
        {
        }
    }

    public class ThemesModel
    {
        public string THEMENAME { get; set; }

        public string THEMEDISPLAY { get; set; }

        public ThemesModel()
        {
        }
    }


}