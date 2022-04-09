using Client_Management.Model;
using Client_Management.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client_Management.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region lokale Variablen

        private DialogService _dialog = new DialogService();

        public static string ResultValue = ""; // statische Variable für die Rückgabe aus dem CommandSelectWindow

        private string _inventoryText = "💻 Inventory     "; // &#128187;
        private string _deploymentText = "📦 Deployment     "; // &#128230;
        private string _jobsText = "📋 Jobs     "; // &#128203;
        private string _groupText = "🍮 Groups     "; // &#127854;

        #region lokale Variablen Deployment

        private string _searchTextCommand;
        private ICommand _removeCommandPackageCommand;
        private ICommand _refreshCommandPackageCommand;
        private Package _selectedPackage;
        private ObservableCollection<Package> _packages = new ObservableCollection<Package>();
        private ObservableCollection<Package> _packagesOriginal = new ObservableCollection<Package>();

        #endregion

        #region lokale Variablen Inventory

        private string _searchText;
        private ICommand _executeComputersCommand;
        private ICommand _refreshComputersCommand;
        private ICommand _computerCommand;
        private ICommand _removeComputerCommand;
        private ICommand _scanComputerCommand;
        private Computer _selectedComputer;
        private Computer _computer;
        private ObservableCollection<Computer> _computers = new ObservableCollection<Computer>();
        private ObservableCollection<Computer> _computersOriginal = new ObservableCollection<Computer>();


        #endregion

        #region lokale Variablen Groups

        private string _searchTextGroup;
        private ICommand _groupCommand;
        private ICommand _groupExecuteCommand;
        private ICommand _removeGroupCommand;
        private ICommand _refreshGroupsCommand;
        private Group _selectedGroup;
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        private ObservableCollection<Group> _groupsOriginal = new ObservableCollection<Group>();

        #endregion

        #region Variablen Jobs

        private ObservableCollection<Job> _jobs = new ObservableCollection<Job>();
        private Job _selectedJob;
        private ICommand _refreshJobsStateCommand;

        #endregion

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public string InventoryText
        {
            get { return _inventoryText; }
            set
            {
                _inventoryText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InventoryText"));
            }
        }
        public string DeploymentText
        {
            get { return _deploymentText; }
            set
            {
                _deploymentText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeploymentText"));
            }
        }

        public string GroupText
        {
            get { return _groupText; }
            set
            {
                _groupText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GroupText"));
            }
        }

        public string JobsText
        {
            get { return _jobsText; }
            set
            {
                _jobsText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("JobsText"));
            }
        }

        #region Deployment Properties

        public string SearchTextCommand
        {
            get { return _searchTextCommand; }
            set
            {
                _searchTextCommand = value;
                Packages = new ObservableCollection<Package>(_packagesOriginal.Where(
                    p =>
                    (p.Name ?? string.Empty).ToLower().Contains((_searchTextCommand ?? string.Empty).ToLower())                  
                    ));
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchText"));
            }
        }

        public ObservableCollection<Package> Packages
        {
            get { return _packages; }
            set
            {
                _packages = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Packages"));
            }
        }
        public Package SelectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                _selectedPackage = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPackage"));
            }
        }

        #endregion

        #region Inventory Properties

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                Computers = new ObservableCollection<Computer>(_computersOriginal.Where(
                    c =>
                    (c.Name ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.Ip ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.Domain ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.Manufacturer ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.SystemFamily ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.Version ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower()) ||
                    (c.Model ?? string.Empty).ToLower().Contains((_searchText ?? string.Empty).ToLower())
                    ));
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchText"));
            }
        }
        public ObservableCollection<Computer> Computers
        {
            get { return _computers; }
            set
            {
                _computers = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Computers"));
            }
        }
        public Computer SelectedComputer
        {
            get { return _selectedComputer; }
            set
            {
                _selectedComputer = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedComputer"));
            }
        }

        #endregion

        #region Group Properties

        public string SearchTextGroup
        {
            get { return _searchTextGroup; }
            set
            {
                _searchTextGroup = value;
                Groups = new ObservableCollection<Group>(_groupsOriginal.Where(
                    g =>
                    (g.Name ?? string.Empty).ToLower().Contains((_searchTextGroup ?? string.Empty).ToLower()) ||
                    (g.Description ?? string.Empty).ToLower().Contains((_searchTextGroup ?? string.Empty).ToLower())
                    ));
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchTextGroup"));
            }
        }

        public ObservableCollection<Group> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Groups"));
            }
        }
        public Group SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGroup"));
            }
        }

        #endregion

        #region Jobs Properties

        public ObservableCollection<Job> Jobs
        {
            get 
            {
                _jobs = new ObservableCollection<Job>(JobsList.GetInstance().JobList);
                return _jobs; 
            }
            set
            {
                _jobs = value;
                JobsList.GetInstance().JobList = new List<Job>(_jobs);
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Jobs"));
            }
        }

        public Job SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedJob"));
            }

        }

        #endregion

        #endregion

        #region Construktoren
        public MainViewModel()
        {
            // nur testen und laden, wenn es überhaupt Sinn macht, d.h. die Einstellungen nicht leer sind
            if (Settings.GetInstance().DbServer != null && Settings.GetInstance().DbUser != null && Settings.GetInstance().DbPassword != null)
            {
                var task = TestConnectionAsync().GetAwaiter();
                task.OnCompleted(() =>
                {
                    // nur fortfahren, wenn der Test erfolgreich war, damit nur eine Fehlermeldung ggf. aufpoppt
                    if (task.GetResult())
                    {
                        _ = RefreshPackagesExecuteAsync();
                        _ = RefreshComputersExecuteAsync();
                        _ = RefreshGroupsExecuteAsync();
                        _ = RefreshJobsStateExecuteAsync();
                    }

                });
            }
        }

        #endregion

        #region Deployment

        #region RefreshCommandPackage
        public ICommand RefreshCommandPackageCommand
        {
            get
            {
                if (_refreshCommandPackageCommand == null)
                {
                    _refreshCommandPackageCommand = new CommandHandler(async () => await RefreshPackagesExecuteAsync(), () => true);
                }
                return _refreshCommandPackageCommand;
            }
        }

        /// <summary>
        /// Aktualisiert die CommandPackage-Liste.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshPackagesExecuteAsync()
        {

            DeploymentText = DeploymentText.Remove(DeploymentText.Length - 4) + "⏳"; 
            Packages = new ObservableCollection<Package>();

            try
            {
                foreach (string p in Directory.GetDirectories(Settings.GetInstance().RepoDir))
                {
                    if(!p.Contains("ReturnMessage"))
                        Packages.Add(new Package(p, Directory.GetCreationTime(p), Directory.GetLastWriteTime(p)));

                }
            }
            catch(DirectoryNotFoundException ex)
            {
                await _dialog.ShowDialogOkAsync("dirNotFoundException:\n\n" + ex.Message, "Directory not found!", DialogType.Error);
            }
            catch (Exception ex)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + ex.Message, "Error!", DialogType.Error);
            }
            _packagesOriginal = Packages;
            DeploymentText = DeploymentText.Remove(DeploymentText.Length - 1) + "    ";
        }

        #endregion

        #region RemoveCommandPackage
        public ICommand RemoveCommandPackageCommand
        {
            get
            {
                if (_removeCommandPackageCommand == null)
                {
                    _removeCommandPackageCommand = new CommandHandler(async () => await RemoveCommandPackageExecuteAsync(), () => RemoveCommandPackageCanExecute());
                }
                return _removeCommandPackageCommand;
            }
        }

        /// <summary>
        /// Löscht das ausgewählte CommandPackage.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveCommandPackageExecuteAsync()
        {
            try
            {
                Directory.Delete(SelectedPackage.Name, true);
            }
            catch (DirectoryNotFoundException ex)
            {
                await _dialog.ShowDialogOkAsync("dirNotFoundException:\n\n" + ex.Message, "Directory not found!", type: DialogType.Error);
            }
            catch (IOException ex)
            {
               await _dialog.ShowDialogOkAsync("IOException:\n\n" + ex.Message, "Cannot operate on directory!", DialogType.Error);
            }
            catch (Exception ex)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + ex.Message, "Error!", DialogType.Error);
            }
            await RefreshPackagesExecuteAsync();
        }

        /// <summary>
        /// Überprüft, ein CommandPackage ausgwählt ist.
        /// </summary>
        /// <returns></returns>
        private bool RemoveCommandPackageCanExecute()
        {
            if (_packages.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Stößt das Öffnen des ausgewählte CommandPackages an.
        /// </summary>
        public void OpenSelected()
        {
            MainWindow.OpenSelected(new CommandPackageWindow(SelectedPackage.Name));
        }


        #endregion


        #region Group

        public ICommand GroupCommand
        {
            get
            {
                if (_groupCommand == null)
                {
                    _groupCommand = new CommandHandler(() => ExecuteGroupCommand(), () => true);
                }
                return _groupCommand;
            }
        }

        /// <summary>
        /// Stößt das Öffnen der ausgewählten Group an.
        /// </summary>
        private void ExecuteGroupCommand()
        {
            MainWindow.OpenSelected(new GroupWindow(SelectedGroup));
        }

        public ICommand GroupExecuteCommand
        {
            get
            {
                if (_groupExecuteCommand == null)
                {
                    _groupExecuteCommand = new CommandHandler(async () => await GroupExecuteAsync(), () => GroupCanExecute());
                }
                return _groupExecuteCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob eine Group ausgewählt ist.
        /// </summary>
        /// <returns></returns>
        private bool GroupCanExecute()
        {
            if (SelectedGroup != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Führt für jede in der ausgewählten Group enthaltenen Computer die Ausführung des im Abfrage-Fenster gewählten Commands aus.
        /// </summary>
        /// <returns></returns>
        private async Task GroupExecuteAsync()
        {
            using (CommandSelectWindow commandSelectWindow = new CommandSelectWindow())
            {
                var result = commandSelectWindow.ShowDialog();
                if (result == true)
                {
                    string val = ResultValue;
                    string command = Path.Combine(Settings.GetInstance().RepoDir, val, "execute.cmd");

                    try
                    {
                        IEnumerable<Computer> tmp = await DatabaseService.GetFilteredComputersAsync(SelectedGroup.Parameter);
                        Parallel.ForEach(tmp, async (currentComputer) =>
                        {
                            await Task.Run(() => WmiService.ExecuteCommand(currentComputer, command));
                        });
                    }
                    catch (Exception e)
                    {
                       await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                    }
                }
            }
        }


        public ICommand RemoveGroupCommand
        {
            get
            {
                if (_removeGroupCommand == null)
                {
                    _removeGroupCommand = new CommandHandler(async () => await RemoveGroupExecuteAsync(), () => RemoveGroupCanExecute());
                }
                return _removeGroupCommand;
            }
        }

        /// <summary>
        /// Löscht die ausgewählte Group.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveGroupExecuteAsync()
        {
            try
            {
                await DatabaseService.RemoveGroupAsync(SelectedGroup);
                await RefreshGroupsExecuteAsync();
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        /// <summary>
        /// Überprüft, ob eine Group ausgewählt ist.
        /// </summary>
        /// <returns></returns>
        private bool RemoveGroupCanExecute()
        {
            if (SelectedGroup != null)
            {
                return true;
            }
            return false;
        }

        public ICommand RefreshGroupsCommand
        {
            get
            {
                if (_refreshGroupsCommand == null)
                {
                    _refreshGroupsCommand = new CommandHandler(async () => await RefreshGroupsExecuteAsync(), () => true);
                }
                return _refreshGroupsCommand;
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der Groups
        /// </summary>
        /// <returns></returns>
        private async Task RefreshGroupsExecuteAsync()
        {
            GroupText = GroupText.Remove(GroupText.Length - 4) + "⏳";
            try
            {
                Groups = new ObservableCollection<Group>(await DatabaseService.GetGroupsAsync());
                _groupsOriginal = Groups;
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
            GroupText = GroupText.Remove(GroupText.Length - 1) + "    ";
        }

        #endregion


        #region Inventory

        public ICommand ExecuteComputersCommand
        {
            get
            {
                if (_executeComputersCommand == null)
                {
                    _executeComputersCommand = new CommandHandler(async () => await CommandHandlerAusgelagert.ExecuteComputersExecuteAsync(SelectedComputer), () => ExecuteComputersCanExecute());
                }
                return _executeComputersCommand;
            }
        }

        /* ist in CommandHandlerAusgelagert.cs umgezogen
         *
        private async Task ExecuteComputersExecuteAsync()
        {
            using (CommandSelectWindow commandSelectWindow = new CommandSelectWindow())
            {
                var result = commandSelectWindow.ShowDialog();
                if(result == true)
                {
                    string val = ResultValue;
                    string _command = Path.Combine(Settings.getInstance().repoDir, val, "execute.cmd");
                    var awaiter = Task.Run(() => WmiService.ExecuteCommand(SelectedComputer, _command));
                    //JobsList.getInstance().JobList.Add(awaiter.Result);
                    await DatabaseService.AddJobAsync(awaiter.Result);
                }
            }
        }
        */

        /// <summary>
        /// Überprüft, ob ein Computer ausgewählt wurde.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem, wie das Ergebnis aussieht.</returns>
        private bool ExecuteComputersCanExecute()
        {
            if (SelectedComputer != null)
            {
                return true;
            }
            return false;
        }

        public ICommand RefreshComputersCommand
        {
            get
            {
                if (_refreshComputersCommand == null)
                {
                    _refreshComputersCommand = new CommandHandler(async () => await RefreshComputersExecuteAsync(), () => true);
                }
                return _refreshComputersCommand;
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der Computer.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshComputersExecuteAsync()
        {
            InventoryText = InventoryText.Remove(InventoryText.Length - 4) + "⏳";

            try
            {
                Computers = new ObservableCollection<Computer>(await DatabaseService.GetComputersAsync());
                _computersOriginal = Computers;
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }

            InventoryText = InventoryText.Remove(InventoryText.Length-1) + "    ";
        }

        public ICommand ComputerCommand
        {
            get
            {
                if (_computerCommand == null)
                {
                    _computerCommand = new CommandHandler(() => ComputerExecute(), () => true);
                }
                return _computerCommand;
            }
        }

        /// <summary>
        /// Stößt das Öffnen des ausgewählten Computers an.
        /// </summary>
        private void ComputerExecute()
        {
            MainWindow.OpenSelected(new ComputerWindow(SelectedComputer));
        }

        public ICommand RemoveComputerCommand
        {
            get
            {
                if (_removeComputerCommand == null)
                {
                    _removeComputerCommand = new CommandHandler(async () => await RemoveComputerExecuteAsync(), () => RemoveComputerCanExecute());
                }
                return _removeComputerCommand;
            }
        }

        /// <summary>
        /// Löscht den ausgwählten Computer.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveComputerExecuteAsync()
        {
            try
            {
                await DatabaseService.RemoveComputerAsync(SelectedComputer);
                await RefreshComputersExecuteAsync();
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        /// <summary>
        /// Überprüft, ob ein Computer ausgewählt ist.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem wie das Ergebnis aussieht.</returns>
        private bool RemoveComputerCanExecute()
        {
            if (SelectedComputer != null)
            {
                return true;
            }
            return false;
        }

        public ICommand ScanComputerCommand
        {
            get
            {
                if (_scanComputerCommand == null)
                {
                    _scanComputerCommand = new CommandHandler(async () => await ScanComputerExecuteAsync(), () => ScanComputerCanExecute());
                }
                return _scanComputerCommand;
            }
        }

        /// <summary>
        /// Stößt das Scannen des ausgewählten Computers an.
        /// </summary>
        /// <returns></returns>
        private async Task ScanComputerExecuteAsync()
        { 
            try
            {
                _computer = await WmiService.QueryAllAsync(SelectedComputer);
                await DatabaseService.AddComputerAsync(_computer);
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        /// <summary>
        /// Überprüft, ob ein Computer ausgewählt wurde.
        /// </summary>
        /// <returns></returns>
        private bool ScanComputerCanExecute()
        {
            if(SelectedComputer!=null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Jobs

        public ICommand RefreshJobsStateCommand
        {
            get
            {
                if (_refreshJobsStateCommand == null)
                {
                    _refreshJobsStateCommand = new CommandHandler( async () => await RefreshJobsStateExecuteAsync(), () => true);
                }
                return _refreshJobsStateCommand;
            }
        }

        /// <summary>
        /// Aktualisiert die Liste der Jobs und prüft den Status der gestarteten Aufgaben bzw. passt diesen ggf. an.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshJobsStateExecuteAsync()
        {
            // soll für jeden Job mit dem State started überprüfen, ob er noch Läuft / wenn beendet, dessen Rückgabe / oder nicht ermittelbar ist.
            // nicht für WMI-Kommandoausführung (create process) in WORKGROUP möglich, da dann kein Zugriff auf Netzlaufwerke o.ä. erlaubt, nur in Domäne und von localhost kann auf Netzlaufwerk zugegriffen werden (d.h. Domäne nicht vollkommen getestet).

            //lösche zu alte Jobs und dazugehörige ReturnMessage-Dateien (älter als eine Woche braucht doch keiner mehr)
            Parallel.ForEach(Jobs.Where(j => j.StartTime < DateTime.Now.AddDays(-7)), async (currentJob) =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        FileDataService.DeleteFile(Path.Combine(Settings.GetInstance().RepoDir, "ReturnMessage", currentJob.ComputerId + "_" + currentJob.StartTime.ToString("yyyyMMddHHmmss")));
                    }
                    catch (Exception e)
                    {
                        //kann man auch nichts machen
                    }
                });

                try
                {
                    await DatabaseService.RemoveJobAsync(currentJob);
                }
                catch (Exception)
                {
                    //ein Satz mit x - das war wohl nix
                    //kann man nichts machen
                }
            });

            //hole alle Jobs (aktualisiert)
            try
            {
                Jobs = new ObservableCollection<Job>(await DatabaseService.GetJobsAsync());
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }

            //überprüfe/aktualisiere den Status jedes Jobs mit dem aktuellem Status started
            Parallel.ForEach(Jobs.Where(j => j.State == Job.States.Started), async (currentJob) =>
            {
                bool chng = false;
                var file = Path.Combine(Settings.GetInstance().RepoDir, "ReturnMessage", currentJob.ComputerId + "_" + currentJob.StartTime.ToString("yyyyMMddHHmmss"));
                if (File.Exists(file))
                {
                    try
                    {
                        if (FileDataService.ReadText(file).Equals("0"))
                        {
                            currentJob.State = Job.States.Finished;
                            chng = true;
                        }
                        else if (FileDataService.ReadText(file).Equals("1"))
                        {
                            currentJob.State = Job.States.Broken;
                            chng = true;
                        }
                    }
                    catch (Exception)
                    {
                        //kann man dan auch nichts machen...
                    }
                }
                else if (currentJob.StartTime < DateTime.Now.AddMinutes(-120))
                {
                    currentJob.State = Job.States.Unknown;
                    chng = true;
                }
                else if (currentJob.ProcessId == -1)
                {
                    currentJob.State = Job.States.Broken;
                    chng = true;
                }

                if (chng)
                {
                    try
                    {
                        await DatabaseService.AddJobAsync(currentJob);
                    }
                    catch (Exception e)
                    {
                        await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                    }
                }
            });

            //lade die nun alle Jobs neu um Änderungen darzustellen
            try
            {
                Jobs = new ObservableCollection<Job>(await DatabaseService.GetJobsAsync());
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        #endregion

        /// <summary>
        /// Testet die Verbindung zur Datenbank.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem wie das Ergebnis aussieht.</returns>
        private async Task<bool> TestConnectionAsync()
        {
            try
            {
                await DatabaseService.TestConnectionAsync();
                return true;
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                return false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
