using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Client_Management.Model;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace Client_Management.Services
{
    class WmiService
    {
        #region Query

        // win32_Process, win32_OperatingSystem, Win32_LogicalDisk, win32_Product, win32_Printer, win32_ComputerSystem, win32_NetworkAdapter(Configuration)

        /* win32_ComputerSystem: Name, PrimaryOwnerName, BootupState, Domain, Manufacturer, Model, SystemFamily, SystemType, UserName
         * win32_OperationSystem: InstallDate, Name, Version, BuildNumber, OSArchitecture, OSLanguage, SerialNumber, Manufacturer, LastBootUpTime
         * win32_Product: Name, IdentifyingNumber, Vendor, Version, InstallDate, InstallSource, LocalPackage, RegOwner, PackageName
         */

        /*
         * Ruft Informationen über das Betriebssystem des angegebenen Computers ab und aktualisiert die damit die Variablen im übergebenen Computer.
         */

        /// <summary>
        /// Ruft Informationen über das Betriebssystem des angegebnenen Computers ab.
        /// Abgefragete Werte sind: InstallDate, Version, BuildNumber, OSArchitecture, OSLanguage, SerialNumber, LastBootUpTime.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>Computer</returns>
        /// <exception cref="CimException">Wird geworfen, wenn etwas mit der CimSession nicht funktioniert (Aufbau/Abfrage).</exception>
        /// <exception cref="Exception">Wird geworfen, wenn es andere Fehler gibt, die nicht direkt von der CimSession verursacht werden.</exception>
        public static Computer QueryOperationSystem(Computer computer)
        {
            string query = "select InstallDate, Version, BuildNumber, OSArchitecture, OSLanguage, SerialNumber, LastBootUpTime from win32_OperatingSystem";
            try
            {

                CimSession cimSession = GetNewCimSession(computer);

                IEnumerable<CimInstance> queryInstances =
                cimSession.QueryInstances(@"root\cimv2", "WQL", query);

                computer = PropertyMapper(computer, queryInstances);

                return computer;
            }
            catch (CimException ex)
            {
                throw new CimException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /* evtl. Alternative zum obigen Query
        private void Test()
        {
            ConnectionOptions cOption = new ConnectionOptions();
            ManagementScope scope = null;
            Boolean isLocalConnection = isLocalhost(machine);

            if (isLocalConnection)
            {
                scope = new ManagementScope(nameSpaceRoot + "\\" + managementScope, cOption);
            }
            else
            {
                scope = new ManagementScope("\\\\" + machine + "\\" + nameSpaceRoot + "\\" + managementScope, cOption);
            }

            if (!String.IsNullOrEmpty(ACTIVE_DIRECTORY_USERNAME) && !String.IsNullOrEmpty(ACTIVE_DIRECTORY_PASSWORD) && !isLocalConnection)
            {
                scope.Options.Username = ACTIVE_DIRECTORY_USERNAME;
                scope.Options.Password = ACTIVE_DIRECTORY_PASSWORD;
            }
            scope.Options.EnablePrivileges = true;
            scope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
            scope.Options.Impersonation = ImpersonationLevel.Impersonate;
            scope.Connect();
        }

        */



        /// <summary>
        /// Ruft Informationen über installierte Software auf dem angegebenen Computer ab und aktualisiert die damit die Variablen im übergebenen Computer.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>Computer</returns>
        /// <exception cref="CimException">Wird geworfen, wenn etwas mit der CimSession nicht funktioniert (Aufbau/Abfrage).</exception>
        /// <exception cref="Exception">Wird geworfen, wenn es andere Fehler gibt, die nicht direkt von der CimSession verursacht werden.</exception>
        public static Computer QueryProduct(Computer computer)
        {
            string query = "select * from win32_Product";
            try
            {
               CimSession cimSession = GetNewCimSession(computer);

                IEnumerable<CimInstance> queryInstances =
                cimSession.QueryInstances(@"root\cimv2", "WQL", query);

                //fehler ab hier
                computer.Products = ProductPropertyMapper(queryInstances);

                return computer;
            }
            catch (CimException ex)
            {
                throw new CimException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Ruft Informationen über das System des angegebenen Computers ab und aktualisiert die damit die Variablen im übergebenen Computer.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>Computer</returns>
        /// <exception cref="CimException">Wird geworfen, wenn etwas mit der CimSession nicht funktioniert (Aufbau/Abfrage).</exception>
        /// <exception cref="Exception">Wird geworfen, wenn es andere Fehler gibt, die nicht direkt von der CimSession verursacht werden.</exception>
        public static Computer QueryComputerSystem(Computer computer)
        {
            string query = "select * from win32_ComputerSystem";
            try
            {
                CimSession cimSession = GetNewCimSession(computer);

                IEnumerable<CimInstance> queryInstances =
                cimSession.QueryInstances(@"root\cimv2", "WQL", query);

                /*
                foreach (CimInstance cimInstance in queryInstances)
                {
                    MessageBox.Show("foreach" + cimInstance);
                    foreach (var enumeratedProperty in cimInstance.CimInstanceProperties)
                    {
                        if (enumeratedProperty.Value != null)
                        {

                            MessageBox.Show(enumeratedProperty.Name + ": " + enumeratedProperty.Value);
                            //Console.WriteLine(enumeratedProperty);
                        }
                    }
                    Console.WriteLine("---------------------");
                }
                */

                computer = PropertyMapper(computer, queryInstances);

                return computer;
            }
            catch (CimException ex)
            { 
                throw new CimException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Generiert eine CimSession für den übergebenen Computer. Wird für die Abfrage von Informationen benötigt.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>CimSession</returns>
        /// <exception cref="CimException">Wird geworfen, wenn etwas mit der CimSession nicht funktioniert (Aufbau).</exception>
        /// <exception cref="Exception">Wird geworfen, wenn es andere Fehler gibt, die nicht direkt von der CimSession verursacht werden.</exception>
        private static CimSession GetNewCimSession(Computer computer) 
        {
            CimSession cimSession;

            try
            {
                CimCredential credentials = new CimCredential(PasswordAuthenticationMechanism.Default, computer.QueryUser.Domain, computer.QueryUser.Username, GetSecureStringPassword(computer.QueryUser.Password));


                if (computer.Ip != "127.0.0.1")
                {
                    WSManSessionOptions sessionOptions = new WSManSessionOptions();
                    sessionOptions.AddDestinationCredentials(credentials);

                    cimSession = CimSession.Create(computer.Ip, sessionOptions);
                }
                else
                {
                    cimSession = CimSession.Create(computer.Ip);
                }
            }
            catch (CimException ex)
            {
                //MessageBox.Show("CimException:\n\n" + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                //return null;
                throw new CimException(ex.Message);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception:\n\n" + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception(ex.Message);
            }
            return cimSession;
        }

        /// <summary>
        /// Ordnet die gesammelten WMI-Informationen (IEnumerable cimInstances) den jeweils passenden Variablen des Computer-Objekts zu.
        /// </summary>
        /// <param name="computer"></param>
        /// <param name="cimInstances"></param>
        /// <returns>Computer</returns>
        private static Computer PropertyMapper(Computer computer, IEnumerable<CimInstance> cimInstances)
        {
            var computerProperties = computer.GetType().GetProperties();

            foreach (CimInstance cimInstance in cimInstances)
            {
                foreach (var cimProperty in cimInstance.CimInstanceProperties)
                {
                    foreach (var computerProperty in computerProperties)
                    {
                        if (computerProperty.Name == cimProperty.Name && cimProperty.Value != null) //normal mit besser, funktioniert in diesem fall aber nicht: && computerProperty.PropertyType == cimProperty.GetType()
                        {
                            if (computerProperty.PropertyType == typeof(DateTime))
                            {
                                DateTime c = (DateTime)cimProperty.Value;
                                //DateTime dt = DateTime.ParseExact(c, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                computerProperty.SetValue(computer, c);
                            }
                            else
                            {
                                computerProperty.SetValue(computer, cimProperty.Value.ToString());
                            }
                        }
                    }
                } 
            }
            return computer;
        }

        /// <summary>
        /// Ordnet die gesammelten WMI-Informationen (IEnumerable CimInstance) den jeweils passenden Variablen eines Product-Objekts zu.
        /// </summary>
        /// <param name="cimInstances"></param>
        /// <returns>List von Product</returns>
        private static List<Product> ProductPropertyMapper(IEnumerable<CimInstance> cimInstances)
        {
            List<Product> products = new List<Product>();
            Product p = new Product();

            var computerProperties = p.GetType().GetProperties();

            foreach (CimInstance cimInstance in cimInstances)
            {
                foreach (var cimProperty in cimInstance.CimInstanceProperties)
                {
                    foreach (var computerProperty in computerProperties)
                    {
                        if (computerProperty.Name == cimProperty.Name) //normal mit besser, funktioniert in diesem fall aber nicht: && computerProperty.PropertyType == cimProperty.GetType()
                        {
                            if (computerProperty.PropertyType == typeof(DateTime))
                            {
                                DateTime dt = DateTime.ParseExact((string)cimProperty.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                                
                                computerProperty.SetValue(p, dt);
                            }
                            else
                            {
                                computerProperty.SetValue(p, cimProperty.Value);
                            }
                        }
                    }
                }
                //mit InstallLocation passt was noch nicht ganz
                products.Add(new Product(p.Name, p.IdentifyingNumber, p.Vendor, p.Version, p.InstallDate, p.InstallSource, p.InstallLocation, p.RegOwner, p.PackageName, p.LocalPackage));
            }
            return products;
        }

        /// <summary>
        /// Führt alle in der WmiService-Classe verfügbaren Abfragen auf den übergebenen Computer aus und aktualisiert dessen Einträge. ACHTUNG: Datenbankeintrag des Computers wird nicht aktualisiert!
        /// </summary>
        /// <param name="computer"></param>
        /// <returns>Computer</returns>
        /// <exception cref="Exception">Wird geworfen, wenn etwas beim Datenbankzugriff oder beim Query der WMI-Informationen fehlschlägt.</exception>
        public static async Task<Computer> QueryAllAsync(Computer computer)
        {
            string wmiEx = "";

            Job job = new Job(computer.Id, "Query", 100000000, 0, DateTime.Now);

            try
            {
                await DatabaseService.AddJobAsync(job);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await Task.Run(() =>
            {
                try
                {
                    computer = QueryComputerSystem(computer);
                    computer = QueryOperationSystem(computer);
                    computer = QueryProduct(computer);
                    job.State = Job.States.Finished;
                }
                catch (CimException e)
                {
                    job.State = Job.States.Broken;
                    wmiEx = e.Message;
                }
                catch (Exception e)
                {
                    job.State = Job.States.Broken;
                    wmiEx = e.Message;
                }
            });
                
            try
            {
                await DatabaseService.AddJobAsync(job);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (job.State == Job.States.Broken)
                throw new Exception(wmiEx);
            return computer;
        }

        #endregion



        #region Execute
        
        /// <summary>
        /// Dient zum starten von Prozessen der CMD in Verbindung mit dem WMIC-Kommando um auf einem Computer einen Prozess zu starten.
        /// Z.B.: ein CMD-Skript oder eine (De)Installation mithilfe von Msiexec oder einfach das Ausführen von exe-Dateien oder Skripten.
        /// </summary>
        /// <param name="computer"></param>
        /// <param name="command"></param>
        public static void ExecuteCommand(Computer computer, string command)
        {
            /*
             * WMIC Return Codes:
             * 0    Success
             * 1    Informational
             * 2    Warning
             * 3    Error
             *
             *
             * WMIC Return Values

             *   Successful completion (0)

             *  Access denied (2)

                Insufficient privilege (3)

                Unknown failure (8)

                Path not found (9)

                Invalid parameter (21)

             * 
             * Wenn WMI einen Fehler zurück geibt, muss das nicht zwingend bedeuten, dass der WMI-Dienst oder einer der WMI-Anbieter ein Problem hat. Fehler können im Teilen des Betriebssystems auftreten und durch WMI weitergereicht werden.
             * Zur Fehlerbehebung/Analyse empfiehlt sich das WMI Diagnostic Utility
             */

            DateTime startTime = DateTime.Now;
            int processId = -1; // Prozess-IDs beginnen bei 0, ein negativer Wert ist somit ungültig und weißt somit auf den Fehlerfall initialisiert
            int returnValue = -1; // wenn die Variable im folgenden nicht gesetzt wird, ist sie bereits auf den Fehlerfall initialisiert
            string wmiccommand;
            Match _match;
            Process p = new Process();

            if(computer.QueryUser.Domain != null)
            {
                p.StartInfo.UserName = computer.QueryUser.Username;
                p.StartInfo.Domain = computer.QueryUser.Domain;
                p.StartInfo.Password = GetSecureStringPassword(computer.QueryUser.Password);
            }

            // es wird keine Shell benötigt und als Anwendung soll die cmd.exe aufgerufen werden
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";

            if(computer.Ip.Equals("127.0.0.1"))
            {
                command += " " + computer.Id + "_" + startTime.ToString("yyyyMMddHHmmss");
                wmiccommand = "/c wmic process call create " + "\"" + command + "\"";
            }
            else if (computer.QueryUser.Domain == null)
            {
                // kann keine uac-Pfade aufrufen!!!
                string unc = "\\\\" + computer.Ip + "\\c$\\Client-MGMT";
                string fileName;
                string destFile;
                string targetPath = Path.Combine(unc, Path.GetDirectoryName(command.Replace(Settings.GetInstance().RepoDir, "")) ?? string.Empty);
                string[] files = Directory.GetFiles(Path.GetDirectoryName(command));

                //Process p1 = Process.Start("net.exe", @"use " + unc + " " + computer.QueryUser.Password + " /USER:" + computer.Ip + "\\" + computer.QueryUser.Username);
                Process p1 = new Process();
                p1.StartInfo.Arguments = @"use " + unc + " " + computer.QueryUser.Password + " /USER:" + computer.Ip + "\\" + computer.QueryUser.Username;
                p1.StartInfo.CreateNoWindow = true;
                p1.StartInfo.UseShellExecute = false;
                p1.StartInfo.FileName = "C:\\Windows\\System32\\net.exe";
                p1.Start();
                p1.WaitForExit();

                // access the files as though they're on \\computername\sharename.
                try
                {
                    Directory.CreateDirectory(targetPath);
                }
                catch(IOException)
                {
                    //k.a. was dann - hauptsache nicht abstürzen
                }

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = Path.GetFileName(s);
                    destFile = Path.Combine(targetPath, fileName);
                    try
                    {
                        File.Copy(s, destFile, true);
                    }
                    catch (IOException)
                    {
                        //k.a. was dann - hauptsache nicht abstürzen
                    }
                }

                // den WMI-Befehl für die cmd zusammenbauen
                string newCommand = Path.Combine(command.Replace(Settings.GetInstance().RepoDir, "C:\\Client-MGMT\\")) + " " + computer.Id + "_" + startTime.ToString("yyyyMMddHHmmss");
                wmiccommand = "/c wmic /user:" + computer.QueryUser.Username + " /password:" + computer.QueryUser.Password + " /node:" + computer.Ip + " process call create " + "\"" + newCommand + "\"";
                // + " " + computer.Id + "_" + startTime.ToString("yyyyMMddHHmmss")

                //net use / delete \\my.server.com
                //Process p2 = Process.Start("net.exe", @"use " + unc + " /delete");
                Process p2 = new Process();
                p2.StartInfo.Arguments = @"use " + unc + " /delete";
                p2.StartInfo.CreateNoWindow = true;
                p2.StartInfo.UseShellExecute = false;
                p2.StartInfo.FileName = "C:\\Windows\\System32\\net.exe";
                p2.Start();
                p2.WaitForExit();
            }
            else
            {
                command += " " + computer.Id + "_" + startTime.ToString("yyyyMMddHHmmss");
                wmiccommand = "/c wmic /node:" + computer.Ip + " process call create " + "\"" + command + "\"";
            }

            p.StartInfo.Arguments = wmiccommand; // Kommando als Prozess-Argument für die cmd.exe setzen
            p.StartInfo.RedirectStandardOutput = true; // Output umleiten
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start(); // Prozess starten

            // Output aufnehmen
            StreamReader reader = p.StandardOutput;
            string output = reader.ReadToEnd();

            // auf Prozess warten und dann Beenden
            p.WaitForExit();
            p.Close();

            //ProzessID und ReturnValue aus dem output des Prozesses p extrahieren
            _match = Regex.Match(output, @"(?<=ProcessId = )\d+");
            if (_match.Success)
            {
                processId = Convert.ToInt32(_match.Value);
            }
            _match = Regex.Match(output, @"(?<=ReturnValue = )\d+");
            if (_match.Success)
            {
                returnValue = Convert.ToInt32(_match.Value);
            }

            try
            {
                _ = DatabaseService.AddJobAsync(new Job(computer.Id, command, processId, returnValue, startTime));
            }
            catch (Exception)
            {
                // schade, kann man nichts machen
            }
            //return new int[] { _processId, _returnValue };
        }

        /// <summary>
        /// Generiert aus dem übergebenen Passwort vom Typ string eins vom Typ SecureString (Wird für den Aufbau der CimSession benötigt).
        /// </summary>
        /// <param name="password"></param>
        /// <returns>SecureString des übergebenen string</returns>
        private static SecureString GetSecureStringPassword(string password)
        {
            SecureString secureStringPassword = new SecureString();
            foreach (char c in password)
                secureStringPassword.AppendChar(c);
            return secureStringPassword;
        }

        #endregion
    }
}
