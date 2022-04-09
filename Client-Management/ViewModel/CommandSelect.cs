using System;
using Client_Management.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Client_Management.Services;
using Package = Client_Management.Model.Package;

namespace Client_Management.ViewModel
{
    class CommandSelect
    {
        #region lokale Variablen

        private DialogService _dialog = new DialogService();

        private ICommand _executeCommand;
        private Package _selectedPackage;
        private ObservableCollection<Package> _packages = new ObservableCollection<Package>();

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
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

        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new CommandHandler(() => ExecuteCommandExecute(), () => ExecuteCommandCanExecute());
                }
                return _executeCommand;
            }
        }

        private bool ExecuteCommandCanExecute()
        {
            if (SelectedPackage != null)
            {
                return true;
            }
            return false;
        }

        private void ExecuteCommandExecute()
        {
            
        }


        #endregion

        #region Constructor
        public CommandSelect()
        {
            Packages = new ObservableCollection<Package>();
            try
            {
                foreach (string p in Directory.GetDirectories(Settings.GetInstance().RepoDir))
                {
                    Packages.Add(new Package(p, Directory.GetCreationTime(p), Directory.GetLastWriteTime(p)));

                }
            }
            catch (DirectoryNotFoundException ex)
            { 
                _ = _dialog.ShowDialogOkAsync("dirNotFoundException:\n\n" + ex.Message, "Directory not found!", DialogType.Error);
            }
            catch (Exception ex)
            {
                _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + ex.Message, "Error!", DialogType.Error);
            }
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
