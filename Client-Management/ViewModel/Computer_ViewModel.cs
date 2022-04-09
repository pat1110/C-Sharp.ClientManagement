using System;
using Client_Management.Model;
using Client_Management.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client_Management.ViewModel
{
    class ComputerViewModel : INotifyPropertyChanged
    {
        #region lokale Variablen

        private DialogService _dialog = new DialogService();

        private ICommand _saveComputerCommand;
        private ICommand _removeComputerCommand;
        private ICommand _scanComputerCommand;
        private ICommand _executeComputerCommand;

        private string _scanStatus = "status: ✔";
        private Computer _computer;
        private System.Net.IPAddress _ip;
        private string _ipString;
        private string _name;
        private string _primaryOwnerName;
        private string _bootupState;
        private string _domain;
        private string _manufacturer;
        private string _model;
        private string _systemFamily;
        private string _systemType;
        private string _userName;

        private DateTime _installDate;
        private string _version;
        private string _buildNumber;
        private string _osLanguage;
        private DateTime _lastBootUpTime;

        private ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private bool _isScanning = false;

        private Credential _selectedCredential;
        private ObservableCollection<Credential> _credentials;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        #region Credentials

        public Credential SelectedCredential
        {
            get { return _selectedCredential; }
            set
            {
                _selectedCredential = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCredential"));
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
        
        #endregion

        #region OperatingSystem

        public string InstallDate
        {
            get { return Convert.ToString(_installDate); }
            set
            {
                _installDate = Convert.ToDateTime(value);
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstallDate"));
            }
        }
        public string Version {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Version"));
            }
        }
        public string BuildNumber 
        {
            get { return _buildNumber; }
            set
            {
                _buildNumber = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuildNumber"));
            }
        }
        public string OSLanguage 
        {
            get { return _osLanguage; }
            set
            {
                _osLanguage = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OSLanguage"));
            }
        }
        public string LastBootUpTime 
        {
            get { return Convert.ToString(_lastBootUpTime); }
            set
            {
                _lastBootUpTime = Convert.ToDateTime(value);
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastBootUpTime"));
            }
        }

        public string Status
        {
            get { return _scanStatus; }
            set
            {
                _scanStatus = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScanStatus"));
            }
        }
        public string Ip {
            get { return _ipString; }
            set
            {
                _ipString = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ip"));
            }
        }

        #endregion

        #region ComputerSystem

        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string PrimaryOwnerName
        {
            get { return _primaryOwnerName; }
            set
            {
                _primaryOwnerName = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrimaryOwnerName"));
            }
        }
        public string BootupState {
            get { return _bootupState; }
            set
            {
                _bootupState = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BootupState"));
            }
        }
        public string Domain {
            get { return _domain; }
            set
            {
                _domain = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Domain"));
            }
        }
        public string Manufacturer {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Manufacturer"));
            }
        }
        public string Model {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Model"));
            }
        }
        public string SystemFamily {
            get { return _systemFamily; }
            set
            {
                _systemFamily = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SystemFamily"));
            }
        }
        public string SystemType {
            get { return _systemType; }
            set
            {
                _systemType = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SystemType"));
            }
        }
        public string UserName {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserName"));
            }
        }
        
        #endregion

        #region Product

        public ObservableCollection<Product> Products {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Products"));
            }
        }

        #endregion

        #endregion

        #region Constructoren
        public ComputerViewModel()
        {
            _computer = new Computer();
            _ = GetCredentialsAsync();
        }
        public ComputerViewModel(Computer computer)
        {
            _computer = computer;

            // Hole alle Credentials aus DB und setze dann die zum Computer gehörende als ausgewählte
            var awaiter = Task.Run(() =>GetCredentialsAsync()).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                try
                {
                    var val = Credentials.Where(c => c.Id.Equals(_computer.QueryUser.Id)).First();
                    SelectedCredential = val;
                }
                catch (Exception ex)
                {
                    _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + ex.Message, "Error!", DialogType.Error);
                }
            });

            //Hole alle Eigenschaften vom Computer in die UI
            TransferFromComputer();
        }

        #endregion

        /// <summary>
        /// Holt die Eigenschaften des Computers vom Objekt und Speichert diese in die Properties für die UI.
        /// </summary>
        private void TransferFromComputer()
        {
            Ip = _computer.Ip;
            SelectedCredential = _computer.QueryUser;
            Name = _computer.Name;
            PrimaryOwnerName = _computer.PrimaryOwnerName;
            BootupState = _computer.BootupState;
            Domain = _computer.Domain;
            Manufacturer = _computer.Manufacturer;
            Model = _computer.Model;
            SystemFamily = _computer.SystemFamily;
            SystemType = _computer.SystemType;
            UserName = _computer.UserName;
            InstallDate = Convert.ToString(_computer.InstallDate);
            Version = _computer.Version;
            BuildNumber = _computer.BuildNumber;
            OSLanguage = _computer.OSLanguage;
            LastBootUpTime = Convert.ToString(_computer.LastBootUpTime);

            if (_computer.Products != null)
                Products = new ObservableCollection<Product>(_computer.Products);
            else
                Products = new ObservableCollection<Product>();
        }

        /// <summary>
        /// Speichert die Computer-Eigenschaften von den Properties der UI in das Computer-Object (lokale Variable _computer).
        /// </summary>
        private void TransferToComputer()
        {
            _computer.Ip = Ip;
            _computer.QueryUser = SelectedCredential;
            _computer.Name = Name;
            _computer.PrimaryOwnerName = PrimaryOwnerName;
            _computer.BootupState = BootupState;
            _computer.Domain = Domain;
            _computer.Manufacturer = Manufacturer;
            _computer.Model = Model;
            _computer.SystemFamily = SystemFamily;
            _computer.SystemType = SystemType;
            _computer.UserName = UserName;
            _computer.InstallDate = Convert.ToDateTime(InstallDate);
            _computer.Version = Version;
            _computer.BuildNumber = BuildNumber;
            _computer.OSLanguage = OSLanguage;
            _computer.LastBootUpTime = Convert.ToDateTime(LastBootUpTime);

            if (_computer.Products != null)
                _computer.Products = new List<Product>(Products);
            else
                _computer.Products = new List<Product>();
        }

        #region SaveComputer
        public ICommand SaveComputerCommand
        {
            get
            {
                if (_saveComputerCommand == null)
                {
                    _saveComputerCommand = new CommandHandler(async () => await SaveComputerExecute(), () => SaveComputerCanExecute());
                }
                return _saveComputerCommand;
            }
        }

        /// <summary>
        /// Speichert/Aktualisiert den Computer in die Datenbank.
        /// </summary>
        /// <returns></returns>
        private async Task SaveComputerExecute()
        {
            TransferToComputer();

            Status = "Status: ⌛";

            try
            {
                await DatabaseService.AddComputerAsync(_computer);
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }

            Status = "status: ✔";
        }

        /// <summary>
        /// Prüft, ob alles notwendige entahlten ist, um den Computer speichern und nutzen zu können.
        /// </summary>
        /// <returns>True/False Typ bool, je nachem, wie das Ergebnis aussieht.</returns>
        private bool SaveComputerCanExecute()
        {
            // nur wenn richtige IP-Address eingetragen
            bool isIp = System.Net.IPAddress.TryParse(_ipString, out _ip);
            return isIp;
        }

        #endregion

        #region RemoveComputer

        public ICommand RemoveComputerCommand
        {
            get
            {
                if (_removeComputerCommand == null)
                {
                    _removeComputerCommand = new CommandHandler(async () => await RemoveComputerExecute(), () => true);
                }
                return _removeComputerCommand;
            }
        }

        /// <summary>
        /// Löscht den Computer aus der Datenbank.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveComputerExecute()
        {
            TransferToComputer();

            try
            {
                await DatabaseService.RemoveComputerAsync(_computer);
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }

            _computer = new Computer();
            await _dialog.ShowDialogOkAsync("Hint:\n\n" + "You can now close the Window if you are sure to delete this entry or you can click the Save-Button if you want to keep the entry anyway.", "Information!", DialogType.Info);
        }

        #endregion

        #region ScanComputer
        public ICommand ScanComputerCommand
        {
            get
            {
                if (_scanComputerCommand == null)
                {
                    _scanComputerCommand = new CommandHandler(async () => await ScanComputerExecute(), () => ScanComputerCanExecute());
                }
                return _scanComputerCommand;
            }
        }

        /// <summary>
        /// Stößst das Scannen des Computer an.
        /// </summary>
        /// <returns></returns>
        private async Task ScanComputerExecute()
        {
            _isScanning = true;
            Status = "Status: ⌛";
            _computer.Ip = _ipString;
            _computer.QueryUser = _selectedCredential;
           
            try
            {
                _computer = await WmiService.QueryAllAsync(_computer);
                //speichere automatisch, damit es möglich ist in der Zwischenzeit das Fenster wieder zu verlassen
                await DatabaseService.AddComputerAsync(_computer);
                //informiere den Benutzer, dass der Computer mit den aktualisieren Eigenschaften automatisch gespeichert wurde
                await _dialog.ShowDialogOkAsync("Information:\n\nThe results of the scan were automatically saved.", "Information!", DialogType.Info);
                
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
            TransferFromComputer();

            Status = "status: ✔";
            _isScanning = false;
        }
        private bool ScanComputerCanExecute()
        {
            // nur wenn nicht bereits Scannt und IP eingetragen ist
            if(!_isScanning && System.Net.IPAddress.TryParse(_ipString, out _ip))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Execute
        public ICommand ExecuteComputersCommand
        {
            get
            {
                if (_executeComputerCommand == null)
                {
                    _executeComputerCommand = new CommandHandler(async () => await CommandHandlerAusgelagert.ExecuteComputersExecuteAsync(_computer), () => true);
                }
                return _executeComputerCommand;
            }
        }


        /* ist in CommandHandlerAusgelagert.cs umgezogen
         * 
        private async Task ExecuteComputersExecuteAsync()
        {
            using (CommandSelectWindow commandSelectWindow = new CommandSelectWindow())
            {
                var result = commandSelectWindow.ShowDialog();
                if (result == true)
                {
                    string val = Main_ViewModel.ResultValue;
                    string _command = Path.Combine(Settings.getInstance().repoDir, val, "execute.cmd");
                    var awaiter = Task.Run(() => WmiService.ExecuteCommand(_computer, _command));
                    //JobsList.getInstance().JobList.Add(awaiter.Result);
                    await DatabaseService.AddJobAsync(awaiter.Result);
                }
            }
        }
        */

        #endregion

        /// <summary>
        /// Holt alle Credentials von der Datenbank und setzt das SelectedCredential-Property auf die erste default Credential.
        /// </summary>
        /// <returns></returns>
        private async Task GetCredentialsAsync()
        {
            
            try
            {
                Credentials = new ObservableCollection<Credential>(await DatabaseService.GetCredentialsAsync());
                var val = Credentials.Where(c => c.Description.Contains("<default>")).First();
                SelectedCredential = val;
            }
            catch (Exception ex)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + ex.Message, "Error!", DialogType.Error);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
