# C-Sharp.ClientManagement

## Allgemeines
Die Anwendung sammelt aktuelle Informationen über die verwalteten Computer (Hardware/Software) und speichert die gesammelten Informationen in einer MySQL-Datenbank. Die Übersicht über die verwaltete Hard- und Software erleichtert es den Überblick zu behalten und ermöglicht es dem Administrator fundierte Entscheidungen zu treffen und ermöglicht somit einen höheren Sicherheitsstandard.
Somit weiß der Administrator genau welche Software in welcher Version auf den Clients mit der unterschiedlichen Hardware installiert ist, welche aktualisiert werden, welche deinstalliert werden und welche noch installiert werden muss. Dadurch ist ebenfalls einfacher eine Aussage über die Kompatibilität von den verwalteten Clients hinsichtlich neuer Software zu treffen.
Dabei ist es äußerst aufwendig jeden Computer einzeln zu warten, was häufig in Sicherheitslücken und einem hohen Arbeitsaufwand resultiert. Aus diesen Gründen ist es auch wichtig, von einem zentralen Punkt aus Installationen und Befehle auf den jeweiligen Clients ausführen zu können.

## Anforderungen an die Datenbank
Bei der Datenbank muss es sich um MySQL handeln. Die angegebenen Anmeldeinformationen müssen zum Erstellen einer Datenbank und Tabellen und zum anschließenden lesen/schreiben/ändern in dieser berechtigen. Zur schnellen Einrichtung bietet sich die Nutzung eines Docker-Containers an. Beigefügt finden Sie meine „docker_compose.yml“.

## Anforderungen an das Repository
Das Repository ist zwingend erforderlich und muss gewisse Eigenschaften haben, um nutzbar zu sein. Es muss sich um einen Ordner oder Laufwerk handeln, das durch eine Netzwerkfreigabe erreichbar ist. Das Repository ist in den Einstellungen der Anwendung zu definierten. Hierbei ist zu beachten, dass die Pfadangabe aus Sicht des zu verwaltenden Computers (Client) zu tätigen ist (z.B. \\srv01\repo\) (man beachte den Backslash am Ende). Ebenfalls ist es notwendig, dass alle Benutzer, die in den Einstellungen als Anmeldeinformationen definiert wurden und für Computer im Inventory verwendet werden Zugriff auf das Repository haben, hierbei sind keine Schreib-Berechtigungen, sondern Lese- und Ausführ-Berechtigung notwendig.
Im Repository soll sich auch ein Ordner „ReturnMessage“ befinden, auf den die Benutzer, die in den Einstellungen als Anmeldeinformationen definiert wurden und für Computer im Inventory verwendet werden Schreib-Zugriff haben (Diese Funktion steht nur in Domänenumgebungen und für den localhost zur Verfügung, d.h. in der Domäne ungetestet).
Drei Beispiele für CommandPackages werden mit beigefügt, diese beiden Ordner müssen lediglich in das Repository-Verzeichnis gelegt werden.
ACHTUNG: Beim Ausführen eines CommandPackages auf dem localhost (127.0.0.1) wird das CommandPackage nicht im Kontext des konfigurierten Benutzers ausgeführt, sondern im Kontext, in dem diese Anwendung läuft (normalerweise keine Admin-Rechte), d.h. unter normalen Umständen keine Installationen möglich (aber z.B. öffnen von „C:\Windows\System32\calc.exe“ ist immer möglich).

Anforderungen an den Client
Auf dem Client (getestet mit Windows 10 (Build: 19042, 18363) und Windows 8 (Build: 9600)) muss folgendes gegeben sein, damit ein Zugriff möglich ist:
- Admin-Benutzer
  - In Gruppe Administratoren
  - In Gruppe Remoteverwaltungsbenutzer
  - In Gruppe WinRMRemoteWMIUsers_
- UAC für Remote-Network-Verbindungen deaktiviert
- Admin-Approval-Mode ggf. deaktivieren (kommt auf das System an)
- TrustedHosts für WSMan auf any (*)
- PSRemoting aktiviert
- Firewall konfigurieren (muss vorhanden und erlaubt sein)
  - DCOM
  - WMI
  - UnsecApp
  - WMI_OUT
  -  Datei- und Druckerfreigabe
- Neustart nach Konfiguration

Dies wurde im beigefügten Skript „la-bomba.cmd“ festgehalten, welches mit Admin-Rechte ausgeführt werden muss.

## Schritte:
- Datenbank und DB-Benutzer mit ausreichend Rechte (am besten root) bereitstellen (siehe Anforderungen an die Datenbank)
- Repository-Verzeichnis bereitstellen (siehe Anforderungen an das Repository)
- Einen abfragbaren Client bereitstellen (siehe Anforderungen an den Client)
- Anwendung starten und Einstellungen konfigurieren

## Verwendete Frameworks und Fremdcode und Bilder:
- LINQ Dynamic Query Library
  - https://docs.microsoft.com/en-us/previous-versions/bb894665(v=msdn.10)?redirectedfrom=MSDN
  - DynamicLync.cs (2138 LoC) wird von GroupWindow_ViewModel verwendet zur dynamischen Filterung (durch Benutzer konfiguriert) (war nicht in normaler Lync-Umgebung verfügbar)
-  Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)
   -  https://www.nuget.org/packages/Microsoft.EntityFrameworkCore
-   Microsoft Management Infrastructure
    -  https://www.nuget.org/packages/Microsoft.Management.Infrastructure/
-   Icon.ico
    -  https://icons-for-free.com/iconfiles/png/512/Box-1320568095448898951.png
