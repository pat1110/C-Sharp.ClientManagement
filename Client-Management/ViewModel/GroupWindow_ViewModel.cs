using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Client_Management.Model;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Client_Management.Services;
using System.Threading.Tasks;

namespace Client_Management.ViewModel
{
    class GroupWindowViewModel : INotifyPropertyChanged
    {
        private DialogService _dialog = new DialogService();

        private ObservableCollection<string> _computerProperties = new ObservableCollection<string>();
        private string _selectedProperty;
        private ObservableCollection<GroupParameter> _items = new ObservableCollection<GroupParameter>();
        private GroupParameter _selectedItem;
        private ObservableCollection<string> _conditions = new ObservableCollection<string>();
        private string _selectedCondition;
        private ObservableCollection<string> _connections = new ObservableCollection<string>();
        private string _selectedConnection;
        private ObservableCollection<Computer> _computers = new ObservableCollection<Computer>();
        private string _searchText;
        private string _name;
        private string _description;
        private Group _group;
        private ICommand _addCommand;
        private ICommand _removeCommand;
        private ICommand _saveCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public GroupWindowViewModel()
        {
            InitializeSelections();
        }

        public GroupWindowViewModel(Group group)
        {
            //DropDown-UI-Elemente bestücken
            InitializeSelections();
            _group = group;
            //Gruppe in die UI holen
            TransferFromGroup();
            //Filter aktualisieren
            _ = RefreshFilteredComputerAsync();

        }

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new CommandHandler(async () => await AddCommandExecuteAsync(), () => AddCommandCanExecute());
                }
                return _addCommand;
            }
        }

        private bool AddCommandCanExecute()
        {
            if (SelectedProperty != null && SelectedCondition != null && SelectedConnection != null && SearchText != null && !SearchText.Equals(""))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Fügt einen neuen Filter-Parameter hinzu.
        /// </summary>
        /// <returns></returns>
        private async Task AddCommandExecuteAsync()
        {
            try
            {
                Items.Add(new GroupParameter(SelectedConnection, SelectedCondition, SelectedProperty, SearchText));
                await RefreshFilteredComputerAsync();
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new CommandHandler(async () => await RemoveCommandExecuteAsync(), () => RemoveCommandCanExecute());
                }
                return _removeCommand;
            }
        }

        /// <summary>
        /// Prüft, ob es möglich ist einen Filterparameter zu löschen und somit ob es sinnvoll ist den Lösch-Button anzubieten.
        /// </summary>
        /// <returns>True/False Typ bool, je nachem, wie das Ergebnis aussieht.</returns>
        private bool RemoveCommandCanExecute()
        {
            if (SelectedItem != null && Items.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Löscht einen Filter-Parameter aus der Liste.
        /// </summary>
        /// <returns></returns>
        private async Task RemoveCommandExecuteAsync()
        {
            try
            {
                Items.Remove(SelectedItem);
                await RefreshFilteredComputerAsync();
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new CommandHandler(async () => await SaveCommandExecuteAsync(), () => SaveCommandCanExecute());
                }
                return _saveCommand;
            }
        }

        /// <summary>
        /// Überprüft, ob alles notwendige zum Speichern/Aktualisieren der Group vorhanden ist.
        /// </summary>
        /// <returns>True/False Typ bool, je nachdem, wie das Ergebnis aussieht.</returns>
        private bool SaveCommandCanExecute()
        {
            if (Items.Count > 0 && Name != null && Description != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Speichert die Group in die Datenbank.
        /// </summary>
        /// <returns></returns>
        private async Task SaveCommandExecuteAsync()
        {
            if (_group != null)
            {
                TransferToGroup();
            }
            else
            {
                _group = new Group(Name, Description, new List<GroupParameter>(Items));
            }

            try
            {
                await DatabaseService.AddGroupAsync(_group);
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        #region Properties

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

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
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchText"));
            }
        }
        public GroupParameter SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }
        public ObservableCollection<GroupParameter> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
            }
        }
        public string SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                _selectedCondition = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCondition"));
            }
        }
        public ObservableCollection<string> Conditions
        {
            get { return _conditions; }
            set
            {
                _conditions = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Conditions"));
            }
        }

        public string SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                _selectedConnection = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedConnection"));
            }
        }
        public ObservableCollection<string> Connections
        {
            get { return _connections; }
            set
            {
                _connections = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Connections"));
            }
        }

        public ObservableCollection<string> ComputerProperties
        {
            get { return _computerProperties; }
            set
            {
                _computerProperties = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ComputerProperties"));
            }
        }
        public string SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                _selectedProperty = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedProperty"));
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

        #endregion


        /// <summary>
        /// Lädt zur Gruppe geöhrenden Filter in die UI.
        /// </summary>
        private void TransferFromGroup()
        {
            Name = _group.Name;
            Description = _group.Description;
            Items = new ObservableCollection<GroupParameter>(_group.Parameter);
        }

        /// <summary>
        /// Speichert den Filter aus der UI in _group (lokale Variable).
        /// </summary>
        private void TransferToGroup()
        {
            _group.Name = Name;
            _group.Description = Description;
            _group.Parameter = new List<GroupParameter>(Items);
        }

        /// <summary>
        /// Bestückt die DropDown-UI-Elemente mit Auswahlmöglichkeiten.
        /// </summary>
        private void InitializeSelections()
        {
            Conditions.Add("LIKE");
            Conditions.Add("NOT LIKE");
            Conditions.Add("IS");
            Conditions.Add("IS NOT");
            SelectedCondition = "LIKE";

            Connections.Add("AND");
            SelectedConnection = "AND";


            foreach (var p in typeof(Computer).GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    ComputerProperties.Add(p.Name);
                }
            }
            foreach (var p in typeof(Product).GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    ComputerProperties.Add("Product." + p.Name);
                }
            }

            SelectedProperty = ComputerProperties.First();
        }

        /// <summary>
        /// Aktualisiert die Liste der Computer, die der Filter abdeckt.
        /// </summary>
        private async Task RefreshFilteredComputerAsync()
        {
            try
            {
                Computers = new ObservableCollection<Computer>(await DatabaseService.GetFilteredComputersAsync(new List<GroupParameter>(Items)));
            }
            catch (Exception e)
            {
                _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
