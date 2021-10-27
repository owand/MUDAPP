namespace MUDAPP.Models.Settings
{
    public class LangModel : ViewModelBase
    {
        public string LANGNAME
        {
            get => language;
            set
            {
                language = value;
                OnPropertyChanged(nameof(LANGNAME));
            }
        }

        public string LANGDISPLAY
        {
            get => langDisplay;
            set
            {
                langDisplay = value;
                OnPropertyChanged(nameof(LANGDISPLAY));
            }
        }

        public string language;
        public string langDisplay;

        public LangModel()
        {
            LANGNAME = null;
            LANGDISPLAY = null;
        }
    }
}