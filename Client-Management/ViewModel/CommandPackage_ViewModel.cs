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
    class CommandPackageViewModel : INotifyPropertyChanged
    {
        #region lokale Variablen

        private DialogService _dialog = new DialogService();

        private string _script = "@echo off" + Environment.NewLine + "cd \"%~dp0\"" + Environment.NewLine + "REM creation: " + DateTime.UtcNow + " by " + Environment.UserName + Environment.NewLine + Environment.NewLine;
        private string _name;
        private bool _saveState;
        private CommandPackage _commandPackage;
        private ICommand _saveCommand;
        private ICommand _addFileCommand;
        private ICommand _removeFileCommand;
        private string _selectedFile;
        private string _saveText = "💾";
        private List<string> _bin = new List<string>();
        private ObservableCollection<string> _files = new ObservableCollection<string>();

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        public ObservableCollection<string> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Files"));
            }
        }
        public string SaveText
        {
            get { return _saveText; }
            set
            {
                _saveText = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SaveText"));
            }
        }
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedFile"));
            }
        }
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
        public string Script
        {
            get { return _script; }
            set
            {
                _script = value;
                OnPropertyChanged();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Script"));
            }
        }

        #endregion

        #region Constructoren
        public CommandPackageViewModel()
        {
            _commandPackage = new CommandPackage();
        }
        public CommandPackageViewModel(string package)
        {
            _commandPackage = new CommandPackage();
            _commandPackage.Name = Path.GetFileName(package);
            Name = _commandPackage.Name;
            try
            {
                Script = FileDataService.ReadText(Path.Combine(Settings.GetInstance().RepoDir, package, "execute.cmd"));
            }
            catch (Exception e)
            {
                _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }
            Files = new ObservableCollection<string>(Directory.GetFiles(Path.Combine(Settings.GetInstance().RepoDir, package)));
            
            if (Files.Any(x => x.Contains("execute.cmd")))
            {
                Files.Remove(Path.Combine(Settings.GetInstance().RepoDir, Name, "execute.cmd"));
            }
        }

        #endregion

        #region AddFile
        public ICommand AddFileCommand
        {
            get
            {
                if (_addFileCommand == null)
                {
                    _addFileCommand = new CommandHandler(async () => await AddFileExecuteAsync(), () => true);
                }
                return _addFileCommand;
            }
        }

        /// <summary>
        /// Handelt alles notwendige zum Auswählen neuer Dateien ab.
        /// </summary>
        /// <returns></returns>
        private async Task AddFileExecuteAsync()
        {
            string[] fileNames = await _dialog.FileDialog("All files (*.*)|*.*|Program files (*.exe;*.jar;)|*.exe;*.jar;|Script files (*.cmd;*.bat;*.sh;)|*.cmd;*.bat;*.sh;");
            foreach (string filename in fileNames)
            {
                if (Files.Any(x => x.Contains(Path.GetFileName(filename))))
                {
                    await _dialog.ShowDialogOkAsync("Error:\n\nThe file " + filename + " already exists in this CommandPackage!", "Error", DialogType.Error);
                }
                else
                {
                    Files.Add(filename);
                }
            }
        }

        #endregion

        #region RemoveFile
        public ICommand RemoveFileCommand
        {
            get
            {
                if (_removeFileCommand == null)
                {
                    _removeFileCommand = new CommandHandler(() => RemoveFileExecute(), () => RemoveFileCanExecute());
                }
                return _removeFileCommand;
            }
        }

        /// <summary>
        /// Verschiebt die ausgewählte Datei in die Mülleimer-Liste (lokale Variable _bin).
        /// </summary>
        public void RemoveFileExecute()
        {
            if (_commandPackage.Name != null)
            {
                _bin.Add(SelectedFile);
            }
            Files.Remove(SelectedFile);
        }

        /// <summary>
        /// Überprüft, ob eine Datei ausgewählt ist. 
        /// </summary>
        /// <returns></returns>
        public bool RemoveFileCanExecute()
        {
            if(_files.Count>0 && SelectedFile!=null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new CommandHandler(async () => await SaveAsync(), () => SaveCanExecute);
                }
                return _saveCommand;
            }
        }

        /// <summary>
        /// Prüft, ob das CommandPackage gespeichert werden kann.
        /// </summary>
        public bool SaveCanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc.
                if (_saveState && _name!=null)
                {
                    return true;
                }
                return false;
            }
        }
        
        /// <summary>
        /// Speichert/Aktualisiert das CommandPackage und kopiert die eingebunden Dateien und löscht die daraus entfernten Dateien (lokale Variable _bin).
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            if (_bin.Count > 0)
            {
                Parallel.ForEach(_bin, async (file) =>
                {
                    await Task.Run(() =>
                     {
                         try
                         {
                             FileDataService.DeleteFile(file);
                         }
                         catch (Exception e)
                         {
                             _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                         }
                     });
                    
                });
            }
            SaveText = "⏳";
            _saveState = false;
            _commandPackage.Name = _name;
            _commandPackage.Script = _script;
            _commandPackage.Files = new List<string>(_files);

            try
            {
                await FileDataService.WriteText(_commandPackage.Script, Path.Combine(Settings.GetInstance().RepoDir, _name, "execute.cmd"));
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
            }

            await Task.Run(() =>
            {
                try
                {
                    FileDataService.CopyFiles(_commandPackage.Files,
                        Path.Combine(Settings.GetInstance().RepoDir, _name));
                }
                catch (Exception e)
                {
                    _ = _dialog.ShowDialogOkAsync("Exception:\n\n" + e.Message, "Error!", DialogType.Error);
                }
            });

            SaveText = "✔";
            await Task.Delay(1000);
            SaveText = "💾";
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _saveState = true;
        }
    }
}
