using System;

namespace Client_Management.Model
{
    public class Settings
    {
        private static Settings _settings;


        private Settings() { }

        static public Settings GetInstance()
        {
            if(_settings==null)
            {
                _settings = new Settings();
            }
            return _settings;
        }
        static public void SetInstance(Settings newSettings)
        {
            _settings = newSettings;
        }
        public String RepoDir { get; set; }
        public String DbServer { get; set; }
        public int DbPort { get; set; }
        public String DbUser { get; set; }
        public String DbPassword { get; set; }

    }
    
}
