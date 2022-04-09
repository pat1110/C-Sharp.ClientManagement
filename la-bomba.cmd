@echo off

echo FOR DE AND EN ONLY!
echo RUN WITH ADMIN-PRIVILEGES!
echo =========================
echo .
echo starting and configuring winRM-Service
net start winRM
sc config winRM start=auto

echo Benutzer erstellen
user Patrick # /add

echo adding user to Admin-Gruppe
net localgroup Administratoren Patrick /add
net localgroup administrators Patrick /add

echo adding user to Remoteverwaltungsbenutzer-Gruppe
net localgroup Remoteverwaltungsbenutzer Patrick /add
net localgroup WinRMRemoteWMIUsers_ Patrick /add
net localgroup "Remote Management Users" Patrick /add

echo disabeling UAC for remote network connections
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v LocalAccountTokenFilterPolicy /t REG_DWORD /d 1

echo disabel the Admin-Approval-Mode if necessary! (see in script)
rem falls notwendig: HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\system\FilterAdministratorToken --> 0

echo setting TrustedHosts to any
powershell -command "Set-Item WSMan:\localhost\Client\TrustedHosts -Value '*' -Force"

echo enable PSRemote
powershell -command "Enable-PSRemoting -force"

REM https://docs.microsoft.com/en-us/windows/win32/wmisdk/connecting-to-wmi-remotely-starting-with-vista
echo adding rules to firewall
netsh advfirewall firewall add rule dir=in name="DCOM" program=%systemroot%\system32\svchost.exe service=rpcss action=allow protocol=TCP localport=135

netsh advfirewall firewall add rule dir=in name ="WMI" program=%systemroot%\system32\svchost.exe service=winmgmt action = allow protocol=TCP localport=any

netsh advfirewall firewall add rule dir=in name ="UnsecApp" program=%systemroot%\system32\wbem\unsecapp.exe action=allow

netsh advfirewall firewall add rule dir=out name ="WMI_OUT" program=%systemroot%\system32\svchost.exe service=winmgmt action=allow protocol=TCP localport=any

echo activating File and Printer Sharing
netsh advfirewall firewall set rule group="File and Printer Sharing" profile=private new enable=Yes
netsh advfirewall firewall set rule group="Datei- und Druckerfreigabe" profile=private new enable=Yes

echo continue to restart... (necessary)
echo restart
pause
shutdown /r /t 5 /f