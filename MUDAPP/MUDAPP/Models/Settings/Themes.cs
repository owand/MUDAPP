namespace MUDAPP.Models.Settings
{
    public class ThemesModel : ViewModelBase
    {
        public string THEMENAME
        {
            get => themeName;
            set
            {
                themeName = value;
                OnPropertyChanged(nameof(THEMENAME));
            }
        }

        public string THEMEDISPLAY
        {
            get => themeDisplay;
            set
            {
                themeDisplay = value;
                OnPropertyChanged(nameof(THEMEDISPLAY));
            }
        }

        public string themeName;
        public string themeDisplay;

        public ThemesModel()
        {
        }
    }
}