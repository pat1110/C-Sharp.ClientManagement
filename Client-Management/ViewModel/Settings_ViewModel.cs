using System;
using Client_Management.Model;
using Client_Management.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client_Management.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        #region lokale Variablen

        private DialogService _dialog = new DialogService();

        private bool _saveState = true;
        private ICommand _saveCommand;
        private ICommand _testCommand;

        #region DB und Repo

        private string _repoDir;
        private string _dbServer;
        private int _dbPort;
        private string _dbUser;
        private string _dbPassword;

        private string _originalRepoDir;
        private string _originalDbServer;
        private int _originalDbPort;
        private string _originalDbUser;
        private string _originalDbPassword;



        //                                               |<-- Zeilenanfang                                                                       IP-Adresse   -->|<--   Hostname                                                                                     -->|   Zeilenende                                                                                     
        private Regex rgxValidServer = new Regex("^((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])|(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9]).)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9-]*[A-Za-z0-9]))$");
        private Regex rgxValidUsername = new Regex("^[a-z0-9__-]+$");

        #endregion

        #region Credentials

        private ICommand _newCredentialCommand;
        private ICommand _removeCredentialCommand;
        private ICommand _refreshCredentialsCommand;
        private ICommand _saveCredentialCommand;

        private string _description;
        private string _username;
        private string _domain;
        private string _password;
        private ObservableCollection<Credential> _credentials = new ObservableCollection<Credential>();
        private Credential _selectedCredential;

        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            // Übernehme die Einstellungen aus der Settings-Instanz in die UI
            RepoDir = Settings.GetInstance().RepoDir;
            DbServer = Settings.GetInstance().DbServer;
            DbPort = Settings.GetInstance().DbPort;
            DbUser = Settings.GetInstance().DbUser;
            DbPassword = Settings.GetInstance().DbPassword;

            // Setze die gerade übernommenen Einstellungen als Original (Wird für TestConnection benötigt)
            _originalRepoDir = RepoDir;
            _originalDbServer = DbServer;
            _originalDbPort = DbPort;
            _originalDbUser = DbUser;
            _originalDbPassword = DbPassword;

            _saveCommand = new CommandHandler(async () => await SaveAsync(), () => SaveCanExecute);

            // Falls DB-Einstellungen vorhanden sind, versuche die Credentials zu laden
            if(DbServer!=null && DbUser!=null && DbPassword != null)
            {
                _ = RefreshCredentialsExecute();
            }
        }

        #region Properties

        #region Credentials

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Username"));
            }
        }

        public string Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Domain"));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
            }
        }

        public ObservableCollection<Credential> Credentials
        {
            get { return _credentials; }
            set
            {
                _credentials = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Credentials"));
            }
        }
        public Credential SelectedCredential
        {
            get { return _selectedCredential; }
            set
            {
                _selectedCredential = value;

                if (_selectedCredential != null)
                {
                    Description = _selectedCredential.Description;
                    Username = _selectedCredential.Username;
                    Domain = _selectedCredential.Domain;
                    Password = _selectedCredential.Password;
                }
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCredential"));
            }
        }

        #endregion

        #region DB und Repo

        public string RepoDir
        {
            get { return _repoDir; }
            set 
            { 
                _repoDir = value; 
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("repoDir"));
            }
        }
        public string DbServer
        {
            get { return _dbServer; }
            set 
            {
                _dbServer = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dbServer"));
            }
        }
        public int DbPort
        {
            get { return _dbPort; }
            set
            {
                _dbPort = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dbPort"));
            }
        }
        public string DbUser
        {
            get { return _dbUser; }
            set 
            { 
                _dbUser = value; 
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dbUser"));
            }
        }
        public string DbPassword
        {
            get { return _dbPassword; }
            set 
            { 
                _dbPassword = value; 
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dbPassword"));
            }
        }

        #endregion


        #endregion

        #region DbSettings Test

        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new CommandHandler(async () => await TestConnectionAsync(), () => TestConnectionCanExecute());
                }
                return _testCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob es überhaupt Sinn macht, die DB-Verbindung zu testen (Sind alle notwendigen Daten eingegeben?).
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem, wie das Ergebnis ist</returns>
        private bool TestConnectionCanExecute()
        {
            if (DbServer != null && DbPort > 0 && DbUser != null && DbPassword != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Testet die Verbindung zur Datenbank und gibt visuelle Rückmeldung.
        /// </summary>
        /// <returns></returns>
        private async Task TestConnectionAsync()
        {
            //Setze die Textfelder als Einstellungen
            TransferToSettings();

            //Teste mit den Einstellungen aus UI
            try
            {
                await DatabaseService.TestConnectionAsync();
                await _dialog.ShowDialogOkAsync("Information:\n\n" + "Connection to Database successfully established!", "Information!", DialogType.Info);

            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
            
            //Setze die Einstellungen wieder auf ihre Originalwerte
            Settings.GetInstance().RepoDir = _originalRepoDir;
            Settings.GetInstance().DbServer = _originalDbServer;
            Settings.GetInstance().DbPort = _originalDbPort;
            Settings.GetInstance().DbUser = _originalDbUser;
            Settings.GetInstance().DbPassword = _originalDbPassword;
            
        }

        #endregion

        #region Settins Save
        public ICommand ClickCommand
        {
            get
            {
                if(_saveCommand == null)
                {
                    _saveCommand = new CommandHandler(async () => await SaveAsync(), () => SaveCanExecute);
                }
                return _saveCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob alles notwenidge an Einstellungen angegeben ist.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem wie das Ergebnis aussieht.</returns>
        public bool SaveCanExecute
        {
            get
            {
                if(_saveState && _repoDir!=null && _dbServer!=null && _dbUser!=null && _dbPassword!=null)
                {
                    if (_repoDir.EndsWith("\\") && Directory.Exists(_repoDir) && rgxValidServer.IsMatch(_dbServer) && _dbPort>0 && _dbPort<65535 && rgxValidUsername.IsMatch(_dbUser) && _dbPassword != "")
                    {
                        return true;
                    }
                }                
                return false;
            }
        }

        /// <summary>
        /// Speichert die Einstellungen in der aktullen Settings-Instanz, setzt sie als neues Original (lokale Variablen) und Speichert sie in der settings.xml.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            _saveState = false;

            // Setze die Textfelder als Einstellungen
            TransferToSettings();

            // Setze die Einstellungen als neue Originalwerte (Wird für TestConnection benötigt)
            _originalRepoDir = Settings.GetInstance().RepoDir;
            _originalDbServer = Settings.GetInstance().DbServer;
            _originalDbPort = Settings.GetInstance().DbPort;
            _originalDbUser = Settings.GetInstance().DbUser;
            _originalDbPassword = Settings.GetInstance().DbPassword;

            //Speichere die neuen Einstellungen in die Datei
            await Task.Run(() => FileDataService.Serialize(Settings.GetInstance(), "settings.xml"));
            
        }

        #endregion

        #region Credentials

        public ICommand NewCredentialCommand
        {
            get
            {
                if (_newCredentialCommand == null)
                {
                    _newCredentialCommand = new CommandHandler(() => NewCredentialExecute(), () => true);
                }
                return _newCredentialCommand;
            }
        }

        /// <summary>
        /// Bereitet die UI für die Eingabe einer neuen Credential vor.
        /// </summary>
        private void NewCredentialExecute()
        {
            Description = null;
            Username = null;
            Domain = null;
            Password = null;
            _selectedCredential = null;
        }

        public ICommand RemoveCredentialCommand
        {
            get
            {
                if (_removeCredentialCommand == null)
                {
                    _removeCredentialCommand = new CommandHandler(async () => await RemoveCredentialExecute(), () => RemoveCredentialCanExecute());
                }
                return _removeCredentialCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob es Sinn macht den Lösch-Button anzubieten.
        /// </summary>
        /// <returns></returns>
        private bool RemoveCredentialCanExecute()
        {
            if (SelectedCredential != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Überprüft, ob noch eine default-Credential vorhanden bleibt und löscht die ausgewählte Credential.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveCredentialExecute()
        {
            if(Credentials.Where(c => c.Description.Contains("<default>")).Count() <= 1 && SelectedCredential.Description.Contains("<default>"))
            {
                await _dialog.ShowDialogOkAsync("Warning:\n\n" + "There must be one Credential with '<default>' in the Description to mark the default!", "Warning!", DialogType.Warning);
            }
            else
            {
                try
                {
                    await DatabaseService.RemoveCredentialAsync(SelectedCredential);
                    await RefreshCredentialsExecute();
                }
                catch (Exception e)
                {
                    await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                }
            }
        }

        public ICommand SaveCredentialCommand
        {
            get
            {
                if (_saveCredentialCommand == null)
                {
                    _saveCredentialCommand = new CommandHandler(async () => await SaveCredentialExecute(), () => SaveCredentialCanExecute());
                }
                return _saveCredentialCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob alle notwendigen Angaben einer Credential enthalten sind.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem, ob alles notwendige Vorhanden ist.</returns>
        private bool SaveCredentialCanExecute()
        {
            if(Description!=null && Username!=null && Password != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fügt eine neue Credential hinzu oder aktualsisiert eine Bestehende.
        /// </summary>
        /// <returns></returns>
        private async Task SaveCredentialExecute()
        {
            try
            {
                await DatabaseService.AddCredentialAsync(new Credential(Description, Username, Domain, Password));
                await RefreshCredentialsExecute();
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
            
            if (Credentials.Where(c => c.Description.Contains("<default>")).Count() > 1)
            {
                await _dialog.ShowDialogOkAsync("Warning:\n\n" + "There already is one default credential specified! This one will be added too - not optimal!", "Warning!", DialogType.Warning);
            }
        }

        public ICommand RefreshCredentialsCommand
        {
            get
            {
                if (_refreshCredentialsCommand == null)
                {
                    _refreshCredentialsCommand = new CommandHandler(async () => await RefreshCredentialsExecute(), () => true);
                }
                return _refreshCredentialsCommand;
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der Credentials.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCredentialsExecute()
        {
            try
            {
                Credentials = new ObservableCollection<Credential>(await DatabaseService.GetCredentialsAsync());
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        #endregion

        /// <summary>
        /// Übernimmt die Benutzereingaben aus der UI in die Einstellungen (Settings-Instanz (Singleton)).
        /// </summary>
        private void TransferToSettings()
        {
            Settings.GetInstance().RepoDir = _repoDir;
            Settings.GetInstance().DbServer = _dbServer;
            Settings.GetInstance().DbPort = _dbPort;
            Settings.GetInstance().DbUser = _dbUser;
            Settings.GetInstance().DbPassword = _dbPassword;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _saveState = true;
        }
        
    }
}
