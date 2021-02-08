# BackupReposetories

# Einleitung

Dies ist ein kleines Programm um alle Repositories eines Azure DevOps-Organisation zu sichern. Hierzu wird im ersten Schritt die Liste aller Projekte und und der dazugehörigen Repositories aufgesammelt (Siehe [Microsoft Dokumentation](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=azure-devops)). Um andere Repository-Quellen zu unterstützen muss das Interface "IRemoteRepositoryService" implementiert werden.

Der Fortschritt wird dokumentiert und standartmäßig in PRTG ausgegeben. Für andere Systeme muss das Interface "IReportService" implementiert werden.

# Installation

## Vorbereitung

- Zunächst muss das Zip **BackupRepositories.zip** mit all seinen Elementen an einen Gewünschten Pfad extrahiert werden.
- Ein **Azure DevOps Token** muss unter **Personal Access Tokens** generiert werden mit den berechtigungen **Code:Read, Code:Status'**
- Ein **Stammverzeichnis** für die Backups muss angelegt werden (Nicht schreibgeschützt).
- Ein Verzeichnis für den **Logoutput** muss angelegt werden (kann auch das Backup **Stammverzeichnis** sein).
- Git sollte installiert werden (wird benötigt für Wiederherstellung)
- Ein Registry Key muss unter Current User: 'SOFTWARE\BackupRepositories\Keys' angelegt werden (siehe appsettings.yml)
  - Es tritt ein Fehler auf wenn der Key nicht existiert

## Konfiguration der Appsettings

Die Konfigurationsdatei **BackupRepositories\appsettings.yml** muss nun wie folgt angelegt werden:

```yml
---
BackupSerilog:
  LogOutputPath: 'C:\Backup\log-{Date}.txt' #AbsoluterPfad\Dateiname-{Date}.txt  
  FileLogOutputTemplate: '[{Timestamp:dd.MM.yyyy HH:mm:ss} {Level:u3}] {Message:j}{NewLine}{Exception}'
  #Template für den FileLogOutput; Siehe Serilog RolingFile Template
  RetainedFileCountLimit: 31 # Tage, nach denen die alten Logfiles überschrieben werden
  ConsoleOutputTemplate: '[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}' # Output Template siehe Serilog Console
  LogLevel: 'Information' # Values: Information, Debug, Verbose;
DataProtection:
  RegistryCurrentUserSubKeyPath: 'SOFTWARE\BackupRepositories\Keys' #Pfad zum Registry Key, indem das Encryption-Secret gespeichert wird
ApplicationConfigurationModel:
  DevOpsAuthConfigurationModel:
    EncryptedAzureDevOpsToken: #Token muss über das Command Encrypt-AzurePat [PlainToken] verschlüsselt werden; Siehe Abschnitt "Ausführung : Verfügbare Befehle"
  DevOpsApiConfigurationModel:
    OrganizationDevOpsUrl: 'https://dev.azure.com/<yourname>' #Url zum DevOps
    OrganizationDevOpsName: <organisationName> #Organisation name des DevOps
  GitConfigurationModel:
    LocalBasePath: 'C:\Backup' #Stammverzeichnis für die Backups
    RemoteOriginName: remoteorigin #Name für den remote in git .config (Nur verändern falls unbedingt nötig)
    FetchRefSpecs: '+refs/*:refs/*' #Branches/Tags die Remote gefetcht werden
  ProjectConfigurationModel:
    MaxProjectsOffset: 999999 #Die maximale Anzahl von DevOps Repositories, die geladen werden (leider erforderlich)   
  PrtgConfigurationModel:
    PrtgPushServerUri: 'http://127.0.0.1:5050' #Protocol://IP:Port des Prtg Network Servers Https Port = 5051 by default Http Port = 5050
    BackupRepositoriesPrtgSensorId: 'FE3C8592-038C-4B2B-B6FE-0CCBB93BB772' #Prtg Sensor Id (Wird bei erstellen des Sensors generiert und kann danach, in den Einstellungen kopiert werden)
```

## Konfiguration Prtg Sensor

- Ein Prtg Sensor vom Typ **HTTP Push Data Advanced** muss angelegt werden.
- In den Einstellungen des Sensors müssen folgende Dinge konfiguriert werden:
  - HTTP Push -> SSL Settings -> HTTP (unsafe)
  - HTTP Push -> Request Method -> POST
  - HTTP Push Data -> No Incoming Data -> Switch to down status after x minutes
  - HTTP Push Data -> Time Threshold (Minutes) -> Die Zeit sollte größer sein als die verzögerung der Backupausführung (ca. 5h mehr muss getestet werden)
- Nachdem der Sensor erzeugt wurde die Einstellungen erneut öffnen und den Identification Token in die appsettings.yml unter BackupRepositoriesPrtgSensorId kopieren:
  - HTTP Push -> Identification Token
- Die "Notification Trigger" im PRTG nach wunsch konfigurieren

## Konfiguration AzureDevOps Token

- Das generierte DevOps Token muss mit dem Import-AzurePat verschlüsselt und importiert werden:

```
Import-AzurePat [PlainToken]
```

# Ausführung


Die Applikation kann entweder mit Startparametern gestartet werden oder Startparameter über den Konsoleninput entgegen nehmen.

Verfügbare Befehle:

```
- Run-BackupRepositories
- Import-AzurePat [PlainToken]
- Eingabe ohne Parameter führt zum Exit [Timeout 60 Sekunden]
```

## Allgemeines Ausführungsverhalten

- Run-BackupRepositories:
    - Erfolgreiche Ausführung:
      - Erzeugt oder erneuert ein lokales Backup
      - Löscht lokale Backups, welche remote nicht mehr existieren
      - Wenn alle Repositories erfolgreich waren: Setzt den Status des PRTG Reports auf OK 
    - Fehlgeschlagene Ausführung (Einzelne Repositories können trotzdem erfolgreich bearbeitet worden sein)
      - Generiert einen Log, indem Details zur Ausführung einzusehen sind
      - Setzt den Status des PRTG Reports auf ERROR mit Messages
    - Generiert einen Log, indem Details zur Ausführung einzusehen sind
    - Sendet einen Report mit Status an den PRTG Server

# Wiederherstellung

## Backup Ordnerstruktur:

```
LocalBasePath
├── ExampleProjectOne
|   ├── ExampleRepositoryOne.git
|   ├── ExampleRepositoryTwo.git
|   └── ...
├── ExampleProjectOne
|   ├── ExampleRepositoryTwo.git
|   └── ...
└── example.log (optional; abhängig von der Konfiguration)
```

## Wiederherstellungsmöglichkeiten

Es gibt nun mehrere möglichkeiten eine Repository wiederherzustellen:

- **Clonen**, von einer Backuprepository in einen lokalen Ordner.
- **Pushen**, auf einen Remoteserver von einer lokalen Backuprepository.

## *Clonen*

```
git clone LocalBasePath\ExampleProjectOne\ExampleRepositoryOne.git Path\To\New\Folder\For\Repository
```
>*Unter Linux oder in GitBash die Backslashes durch Slashes ersetzen!*

> Da alle Backups "--bare" bzw. "--mirror" Repositories sind, haben sie keinen Workingtree. Das heißt, es ist nicht möglich, direkt in einen Backup "git checkout" auszuführen. Durch das Clonen von einem Backup wird eine "normale" Repository erzeugt, die dann auch einen lokalen Workingtree hat, was dann "git checkout" ermöglicht.

## *Pushen*

- Zunächst muss eine Repository auf einem Remoteserver angelegt werden.
- Die Remote-Server-Repository muss leer sein.
- Die Remote-Server-Repository sollte eine "--bare" Repository sein (Wird automatisch bei Github/ DevOps gemacht), damit von dort wieder clone ausgeführt werden kann.

```
cd LocalBasePath\ExampleProjectOne\ExampleRepositoryOne.git
git remote add --mirror=push restoretarget <targetUrl>
git push --mirror restoretarget
```

# Limitierungen

Es können keine Repositories gesichert werden, die inneinander verschachtelt sind.

# Contribution

Pull-Requests sind herzlich willkommen. Es gelten die [bluehands development-guidelines](https://github.com/bluehands/development-guidelines)