
namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow
    {
        public HelpWindow()
        {
            InitializeComponent();

            string webPage = $@"
<!doctype html>
<html lang=""en"">
<head>
     <meta charset = ""utf-8"">
    <style>
        html * {{ font-family: Segoe UI !important; font-size: 12px !important; }}
        h1 {{ border-bottom: 2px solid rgb(23, 154, 200); background-color: lightblue; }}
        h2 {{ background-color: lightblue; }}
        div {{ border: 1px solid grey; }}
        p {{ font-weight: normal; }}
    </style>
</head>

<div>
    <h1>Über:</h1>
    <h2>Autor: https://github.com/pat1110/</h2>
    <h2>Version: 1.0</h2>
    <h2>Winter Build</h2>
    <h3>confianza en el éxito</h3>
    <h1>Verwendete Frameworks und Fremdcode und Bilder:</h1>
    <ul>
        <li>LINQ Dynamic Query Library
            <ul>
                <li>https://docs.microsoft.com/en-us/previous-versions/bb894665(v=msdn.10)?redirectedfrom=MSDN</li>
            </ul>
        </li>
        <li>Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)
            <ul>
                <li>https://www.nuget.org/packages/Microsoft.EntityFrameworkCore</li>
            </ul>
        </li>
        <li>Microsoft Management Infrastructure</li>
            <ul>
                <li>https://www.nuget.org/packages/Microsoft.Management.Infrastructure/</li>
            </ul>
        <li>Icon.ico</li>
            <ul>
                <li>https://icons-for-free.com/iconfiles/png/512/Box-1320568095448898951.png</li>
            </ul>
    </ul>
</div>
<h3>Inhaltsverzeichnis</h3>

<div>
    <h3><a href = ""#Allgemeines"">Allgemeines</a></h3>
    <h3><a href = ""#Verwalten von Command-Packages"">Verwalten von Command-Packages</a><h3>
    <h3><a href = ""#Verwalten von Computern"">Verwalten von Computern</a></h3>
    <h3><a href = ""#Schnellstartanleitung"">Schnellstartanleitung</a></h3>
</div>

<a name=""Allgemeines""/>
<h1>Allgemeines</h1>
<p>
Bei dieser Anwendung handelt es sich um ein Programm, das zum verwalten von (Windows) Computern dienen soll. Es ermöglicht die Installation von Programmen sowie die Abfrage von Informationen über einen Computer über das Netzwerk. Hierfür muss neben der IP-Adresse auch ein Benutzer mit den nötigen Rechten für den Computer bekannt sein.
</p>

<h2>Repository</h2>
<p>
Das Repository ist zwingend erforderlich und muss gewisse Eigenschaften haben, um nutzbar zu sein. Es muss sich um einen Ordner oder Laufwerk handeln, das durch eine Netzwerkfreigabe erreichbar ist. Das Repository ist in den Einstellungen der Anwendung zu definierten. Hierbei ist zu beachten, dass die Pfadangabe aus Sicht des zu verwaltenden Computers (Client) zu tätigen ist (z.B. \\srv01\repo\). Ebenfalls ist es notwendig, dass alle Benutzer, die in den Einstellungen als Anmeldeinformationen definiert wurden und für Computer im Inventory verwendet werden zugriff auf das Repository haben, hierbei sind keine Schreib-Berechtigungen notwendig.</br>
Im Repository soll sich auch ein Ordner „ReturnMessage“ befinden, auf den die Benutzer, die in den Einstellungen als Anmeldeinformationen definiert wurden und für Computer im Inventory verwendet werden Schreib-Zugriff haben (Diese Funktion steht nur in Domänenumgebungen und für den localhost zur Verfügung, d.h. in der Domäne ungetestet).
</p>

<h2>Datenbank</h2>
<p>
Bei der Datenbank muss es sich um MySQL oder MariaDB handeln. Die angegebenen Anmeldeinformationen müssen zum Erstellen einer Datenbank und zum anschließenden lesen/schreiben/ändern in dieser berechtigen.
</p>

<h2>Deployment</h2>
<p>
Im Deployment-Tab verwaltet man Command-Packages. Darunter versteht man eine Sammlung von Dateien, die zur Ausführung einer Aufgabe (z.B. Installation) benötigt werden und hierfür zu einem Command-Package geschnürt wurden. Der Kontrollfluss des Command-Packages wird durch eine Batch-Datei dargestellt.</br>
Des Weiteren kann der Erfolg/Fehlschlag durch eine ReturnMessage-Datei bestimmt werden, in dieser Datei darf nur 0 oder 1 stehen, entspricht es nicht genau dem, dann wird es nicht beachtet, d.h. den Befehl 'echo|set /p=""0"" > \\{{RepoDir}}\ReturnMessage\%1' verwenden (RepoDir ist ein Platzhalter für den korrekten Pfad in diesem Beispiel).</br>
ACHTUNG: Beim Anwenden eines Command-Packages auf den localhost (127.0.0.1) wird das Package nicht im Kontext des Konfigurierten Benutzers ausgeführt, sondern im aktuellen Kontest dieser Anwendung d.h. Installationen normalerweise nicht möglich, da die Anwendung sonst mit Admin-Rechten gestartet werden müsste.
</p>

<h2>Inventory</h2>
<p>
Im Inventory-Tab verwaltet man die Computer. Hierüber werden die Informationen zu einem Computer zugänglich gemacht. Dies ist für einen Administrator wichtig, um sich über Computer zu informieren und dementsprechend anschließend Entscheidungen treffen zu können.
</p>

<h2>Jobs</h2>
<p>
Im Jobs-Tab kann man sich Einsicht über die bisher auszuführenden Kommandos erlangen. Hier werden angestoßene Aufträge festgehalten, wodurch die Nachvollziehbarkeit der Tätigkeiten erleichtert werden soll. 
Wird von einem Commando eine ReturnMessage abgespeichert unter {{RepoDir}}\ReturnMessage\{{PC-Id_Timestamp}} (PC-Id_Timestamp wird durch wmi-Aufruf an Skript übergeben (%1)), wird das für den Status verwendet (0 entspricht finished, 1 entspricht broken) (dies ist nur in einer Domäne und für localhost möglich, nicht für WORKGROUP, da diese nicht aus dem WMI-Prozess direkt auf Netzlaufwerke zugreifen kann).
In der ReturnMessage-Datei darf nur 0 oder 1 stehen, entspricht es nicht genau dem, dann wird es nicht beachtet, d.h. den Befehl 'echo|set /p=""0"" > \\{{RepoDir}}\ReturnMessage\%1' verwenden (RepoDir ist ein Platzhalter für den korrekten Pfad in diesem Beispiel).
Wird 120 Minuten lang keine ReturnMessage gefunden, dann wächselt der Status auf ""unknown"". Ist der Code 1 wächselt der Status auf ""broken"", ist der Code 0 wächselt der Status auf ""finished"". 
Ist die WMI-ProdessId -1 wächselt der Status umgehend auf ""broken"".
</p>

<h2>Einstellungen</h2>
<p>
Die Einstellungen sind in zwei Teile aufgeteilt. 
Der eine Teil dient der Grundkonfiguration, um eine Verbindung zur Datenbank herstellen zu können und um ein Repository zu definieren. 
Der andere Teil dient zur Verwaltung der Credentials, also der Anmeldeinformationen. Diese werden zum Abfragen und zum Ausführen von Befehlen auf den Computern benötigt, wobei ein '<default>' in der Beschreibung der Anmeldeinformation den Standardwert kennzeichnet, der beim Anlegen eines neuen Computers vorausgewählt ist. Sind mehrere Anmeldeinformationen als Standard gekennzeichnet, wird die erst beste gewählt (dies wird aber versucht zu vermeiden). Wird das Feld Domain ausgefüllt, muss beachtet werden, dass des sich um die Domain handelt, in der sich der Server selbst auch befindet. (Domain-Funktion in Test-Phase).</br>
</p>

<a name=""Verwalten von Command-Packages""/>
<h1>Verwalten von Command-Packages</h1>
<h2>Anlegen eines Commands</h2>
<p>
Hierfür auf ✨ klicken. Nun öffnet sich ein Fenster, worin alle Eigenschaften des Command-Packages definiert werden. Der Name des Command-Packages muss eindeutig sein. Durch + und - können die benötigten Dateien hinzugefügt und entfernt werden. Der Kontrollfluss des Command-Packages wird über eine Batch-Datei gesteuert, deren Inhalt wird in der großen Textbox in der unteren Hälfte des Fensters dargestellt.  
</br>
</br>
Folgendes wird bei einem neuen Command standardmäßig bereits angegeben:</br>
</br>
@echo off</br>
cd ""%~dp0""</br>
REM creation: &ltDatum&gt &ltUhrzeit&gt by &ltErsteller&gt</br>
</br>
Der Eintrag ""@echo off"" bewirkt, dass nur Tatsächliche Ausgaben geschrieben werden, Befehle werden nicht ausgegeben.</br>
Der Eintrag 'cd ""%~dp0""' ist wichtig, damit das Arbeitsverzeichnis auf das Verzeichnis der Batch - Datei gesetzt wird.</br>
Wurde eine Datei hinzugefügt zu dem CommandPackage kann diese durch den Dateinamen aufgerufen werden z.B.: call Setup.exe.</br>
Um den Status der Ausführung zurückzugeben kann folgendes getan werden:</br>
für Erfolg:</br>
echo|set /p=""0"" > \\server\repo-dir\ReturnMessage\%1</br>
für Fehler:</br>
echo|set /p=""0"" > \\server\repo-dir\ReturnMessage\%1</br>

</p>

<h2>Bearbeiten eines Commands</h2>
<p>
Zum Bearbeiten eins Command-Packages genügt ein Doppelklick. Hierdurch wird das Fenster zu bearbeiten des Command-Package aufgerufen, anschließend gilt dasselbe, wie beim Anlegen eines Commands.
</p>

<h2>Löschen eines Commands</h2>
<p>
Zum Löschen eins Command-Packages muss dieses ausgewählt werden und anschließend auf das rote X geklickt werden. Hierdurch wird es gelöscht. (ACHTUNG: Es gibt keine Papierkorb-Funktion in der Anwendung)
</p>

<a name=""Verwalten von Computern""/>
<h1>Verwalten von Computern</h1>
<h2>Computer darauf vorbereiten</h2>
<p>
Jeder Computer muss darauf vorbereitet werden, aus der Ferne mithilfe von WMI (Windows Management Interface) verwaltet zu werden. 
Dieser initiale Schritt ist zwingend erforderlich. Befinden sich alle Systeme in derselben Domäne, ist dies einfach zu gewährleisten, hierfür genügen dann ein Domänen-Administrator-Konto und eine Gruppenrichtlinie.
Befinden sich die Computer jedoch nicht in einer Domäne gestaltet sich dies etwas schwieriger. Für diesen Fall muss das Netzwerkprofil auf ""Privat"" festgelegt werden und naschließend kann das Skript ""la-bomba.cmd"" genutzt werden (als Administrator ausführen!), worin alle notwendigen Schritte, um einen Computer darauf vorzubereiten definiert sind. Der zwingend erforderliche Neustart wird erst nach Bestätigung angestoßen. 
Alle Befehle, die in dem Skript enthalten sind, müssen mit administrativen Rechten ausgeführt werden. Am Schluss der Konfiguration ist ein Neustart des Systems notwendig.
ERINNERUNG: Vor der Ausführung des Skripts ist darauf zu achten, dass der Computer sich nicht in einem als öffentliches Netzwerk verbundenen Netzwerkverbindung befindet zulässig sind an dieser Stelle Privat oder Domäne (Domäne: ungetestet!).
Die administrative Freigabe C$ muss auf dem Client zugreifbar sein, dies wird normalerweise durch das Skript sichergestellt (Datei- und Druckerfreigabe muss aktiv sein).
</p>

<h2>Anlegen eines Computers</h2>
<p>
Hierfür auf ✨ klicken. Anschließend öffnet sich ein Fenster, in dem die IP-Adresse des zu überwachenden Systems angegeben werden muss und für das System valide Anmeldeinformationen ausgewählt werden muss. Ist dies geschehen kann ein Scan des Computers durchgeführt werden und/oder gespeichert werden.
</p>

<h2> Bearbeiten / Betrachten eines Computers</h2>
<p>
Hierfür genügt ein Doppelklick auf den Computer. Zu beachten ist, dass die durch WMI gewonnenen Informationen nur betrachtet und nicht verändert werden können.
</p>
     

<h2> Scannen eines Computers</h2>
<p>
Zum Scannen eines Computers muss einer ausgewählt werden und anschließend auf die blaue 🔎, wodurch umgehend der Scan begonnen wird. Der Auftrag wird im Jobs-Tab angezeigt.
</p>
          

<h2> CommandPackage auf einem Computer ausführen</h2>
<p>
Hierfür muss zunächst ein Computer ausgewählt sein. Nun kann die Schaltfläche ⚡ betätigt werden, wodurch ein Fenster zur Auswahl eins Command-Packages geöffnet wird. Nach bestätigen eines Command-Packages wird die Aufgabe gestartet und im Jobs-Tab angezeigt.
</p>
                 

<h2> Löschen eines Computers</h2>
<p>
Hierfür muss ein Computer ausgewählt sein, ist dies erfüllt kann der Computer durch einen Klick auf das rote X gelöscht werden. (ACHTUNG: Es gibt keine Papierkorb - Funktion in der Anwendung)
</p>

<h1>Verwalten von Gruppen</h1>
<h2>Anlegen einer Gruppe</h2>
<p>
Hierfür auf ✨ klicken. Anschließend öffnet sich ein Fenster, in dem ein Name und eine Beschreibung der neuen Gruppe angegeben werden muss. Ist dies geschehen können verschiedene Filter auf die Liste der Computer angewandt werden, das Filterergebnis wird direkt darunter ausgegeben.
</p>

<h2> Bearbeiten / Betrachten einer Gruppe</h2>
<p>
Hierfür genügt ein Doppelklick auf die Gruppe, anschließend kann die Gruppe bearbeitet werden.
</p>
          

<h2> CommandPackage auf eine Gruppe ausführen</h2>
<p>
Hierfür muss zunächst eine Gruppe ausgewählt sein. Nun kann die Schaltfläche ⚡ betätigt werden, wodurch ein Fenster zur Auswahl eins CommandPackages geöffnet wird. Nach bestätigen eines CommandPackages wird für jeden in der Gruppe enthaltenen Computer die Aufgabe gestartet und im Jobs-Tab angezeigt.
</p>
                 

<h2> Löschen einer Gruppe</h2>
<p>
Hierfür muss eine Gruppe ausgewählt sein, ist dies erfüllt kann die Gruppe durch einen Klick auf das rote X gelöscht werden. (ACHTUNG: Es gibt keine Papierkorb-Funktion in der Anwendung)
</p>

<a name=""Schnellstartanleitung""/>
<h1> Schnellstartanleitung </h1>           

<h2> Datenbank vorbereiten </h2>
<p>
Hierfür muss Docker auf dem Computer installiert sein (getestet mit Version 2.4.0.0 (48506) Channel stable).</br>
</br>
<u>docker-compose.yml</u>
</br>
version: '3.1'</br>
services:</br>
&nbsp;&nbsp;db:</br>
&nbsp;&nbsp;&nbsp;&nbsp;image: mysql</br>
&nbsp;&nbsp;&nbsp;&nbsp;container_name: mysql_Client-Management</br>
&nbsp;&nbsp;&nbsp;&nbsp;restart: always</br>
&nbsp;&nbsp;&nbsp;&nbsp;environment:</br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MYSQL_ROOT_PASSWORD: password</br>
&nbsp;&nbsp;&nbsp;&nbsp;ports:</br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- ""3307:3306""</br>
</br>
CMD in dem Verzeichnis, wo die YML-Datei abgelegt wurde öffnen.</br>
Den Befehl ""docker-compose up"" ausführen.
</p>
<h2> Repository vorbereiten </h2>
<p>
Hierfür muss ein Verzeichnis ausgewählt werden, für die Anforderungen an dieses Verzeichnis siehe Abschnitt <a href = ""#Allgemeines"">Allgemeines</a> --&gt Repository.
</p>
<h2> Einstellungen setzen </h2>
<p>
Wenn die Anwendung geöffnet wird, müssen zuerst die Einstellungen hinterlegt werden (DB, Repo, Credentials (eine Credential muss mit <default> in der Beschreibung als Standard gekennzeichnet werden)).
</p>
<h2> Client vorbereiten </h2>
<p>
Hierzu am Client anmelden, das Netzwerk-Profil auf ""Private"" setzen und mit Admin-Rechten das Skript ""la-bomba.cmd"" ausführen.
Anschließend kann bereits der erste Computer im Inventory aufgenommen und gescannt werden.</br>
</br>
<u>la-bomba.cmd</u></br>
@echo off</br>
</br>
echo FOR DE AND EN ONLY!</br>
echo RUN WITH ADMIN-PRIVILEGES!</br>
echo =========================</br>
echo .</br>
echo starting and configuring winRM-Service</br>
net start winRM</br>
sc config winRM start=auto</br>
</br>
echo Benutzer erstellen</br>
user Patrick # /add</br>
</br>
echo adding user to Admin-Gruppe</br>
net localgroup Administratoren Patrick /add</br>
net localgroup administrators Patrick /add</br>
</br>
echo adding user to Remoteverwaltungsbenutzer-Gruppe</br>
net localgroup Remoteverwaltungsbenutzer Patrick /add</br>
net localgroup WinRMRemoteWMIUsers_ Patrick /add</br>
net localgroup ""Remote Management Users"" Patrick /add</br>
</br>
echo disabeling UAC for remote network connections</br>
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System / v LocalAccountTokenFilterPolicy / t REG_DWORD / d 1</br>
</br>
echo disabel the Admin - Approval - Mode if necessary!(see in script)</br>
    rem falls notwendig: HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\system\FilterAdministratorToken-- > 0</br>
</br>
echo setting TrustedHosts to any</br>
powershell - command ""Set-Item WSMan:\localhost\Client\TrustedHosts -Value '*' -Force""</br>
</br>
echo enable PSRemote</br>
powershell - command ""Enable-PSRemoting -force""</br>
</br>
REM https://docs.microsoft.com/en-us/windows/win32/wmisdk/connecting-to-wmi-remotely-starting-with-vista</br>
echo adding rules to firewall</br>
netsh advfirewall firewall add rule dir =in name = ""DCOM"" program =% systemroot %\system32\svchost.exe service = rpcss action = allow protocol = TCP localport = 135</br>
</br>
netsh advfirewall firewall add rule dir =in name = ""WMI"" program =% systemroot %\system32\svchost.exe service = winmgmt action = allow protocol = TCP localport = any</br>
</br>
netsh advfirewall firewall add rule dir =in name = ""UnsecApp"" program =% systemroot %\system32\wbem\unsecapp.exe action = allow</br>
</br>
netsh advfirewall firewall add rule dir =out name = ""WMI_OUT"" program =% systemroot %\system32\svchost.exe service = winmgmt action = allow protocol = TCP localport = any</br>
</br>
echo activating File and Printer Sharing</br>
netsh advfirewall firewall set rule group = ""File and Printer Sharing"" profile =private new enable=Yes</br>
   netsh advfirewall firewall set rule group=""Datei- und Druckerfreigabe"" profile=private new enable=Yes</br>
</br>
   echo continue to restart... (necessary)</br>
   echo restart</br>
shutdown /r /t 5 /f</br>
</p>


</ html >

                                ";


            Webbrowser.NavigateToString(webPage);


        }
    }
}
