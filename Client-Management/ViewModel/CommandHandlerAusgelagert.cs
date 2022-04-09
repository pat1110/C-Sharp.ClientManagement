using Client_Management.Model;
using Client_Management.Services;
using System.IO;
using System.Threading.Tasks;

namespace Client_Management.ViewModel
{
    class CommandHandlerAusgelagert
    {
        /// <summary>
        /// Stößt die Ausführung des CommandPackages auf dem ausgewählten Computer an.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns></returns>
       public static async Task ExecuteComputersExecuteAsync(Computer computer)
        {
            using (CommandSelectWindow commandSelectWindow = new CommandSelectWindow())
            {
                var result = commandSelectWindow.ShowDialog();
                if (result == true)
                {
                    string val = MainViewModel.ResultValue;
                    string command = Path.Combine(Settings.GetInstance().RepoDir, val, "execute.cmd");
                    await Task.Run(() => WmiService.ExecuteCommand(computer, command));
                }
            }
        }
    }
}
