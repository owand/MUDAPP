using MUDAPP.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MUDAPP.Models.Settings
{
    public class Settings : ViewModelBase
    {
        public ObservableCollection<LangModel> LangCollection { get; }
        public ObservableCollection<ThemesModel> ThemesCollection { get; }

        public ContentList _SelectedItem;
        public List<ContentList> ContentList { get; set; }

        public Settings()
        {
            LangCollection = new ObservableCollection<LangModel>()
            {
                new LangModel { LANGDISPLAY = AppResource.LanguageRus, LANGNAME = "ru" },
                new LangModel { LANGDISPLAY = AppResource.LanguageEng, LANGNAME = "en" }
            };

            ThemesCollection = new ObservableCollection<ThemesModel>()
            {
                new ThemesModel { THEMEDISPLAY = AppResource.ThemesDarkName, THEMENAME = "myDarkTheme" },
                new ThemesModel { THEMEDISPLAY = AppResource.ThemesLightName, THEMENAME = "myLightTheme" },
                new ThemesModel { THEMEDISPLAY =  AppResource.ThemesOSName, THEMENAME = "myOSTheme" }
            };

            ContentList = new List<ContentList>()
            {
            new ContentList() { Title = AppResource.TitleMudCatalog, TargetName = nameof(Views.Mud.MudCatalogPage) },
            new ContentList() { Title = AppResource.TitleMudType, TargetName = nameof(Views.Mud.MudTypePage) },
            new ContentList() { Title = AppResource.TitleCalcCementVol, TargetName = nameof(Views.Mud.CalcCementVolPage) },
            new ContentList() { Title = AppResource.TitleCalcCementWeight, TargetName = nameof(Views.Mud.CalcCementWeitPage) },
            new ContentList() { Title = AppResource.TitleCalcSpacerVol, TargetName = nameof(Views.Mud.CalcSpacerVolPage) }
            };

        }


        public ContentList Selecteditem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                //if (_SelectedItem != null)
                //{
                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        await Shell.Current.GoToAsync(_SelectedItem.TargetName);
                //        //_SelectedItem = null;
                //    });
                //}
            }
        }

    }


    public class ContentList : ViewModelBase
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public ShellNavigationState TargetName { get; set; }

        public ContentList()
        {
        }
    }




}