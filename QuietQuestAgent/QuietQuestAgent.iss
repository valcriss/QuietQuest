; -----------------------------------------------------------------------------
; Inno Setup Script for QuietQuest Agent Installer (All Users)
; -----------------------------------------------------------------------------

; ---------------------------------
; Préprocesseur
; ---------------------------------
; Définit le dossier de sortie du build (modifie selon ta configuration)
#define MySourceDir "bin\release\net9.0-windows\publish\win-x64"

[Setup]
AppName=QuietQuest Agent
AppVersion=1.0.0
; Installation pour tous les utilisateurs dans Program Files
DefaultDirName={commonpf}\QuietQuestAgent
DefaultGroupName=QuietQuest Agent
OutputBaseFilename=QuietQuestAgentInstaller
Compression=lzma2
SolidCompression=yes
; Droits administrateur requis
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64os

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Executable principal et dépendances
Source: "{#MySourceDir}\\QuietQuestAgent.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

; Assets (si nécessaire)
Source: "{#MySourceDir}\\Assets\\*"; DestDir: "{app}\\Assets"; Flags: ignoreversion recursesubdirs createallsubdirs


[Registry]
; Ajout au démarrage automatique pour tous les utilisateurs
Root: HKLM; Subkey: "Software\\Microsoft\\Windows\\CurrentVersion\\Run"; ValueType: string; ValueName: "QuietQuestAgent"; ValueData: """{app}\\QuietQuestAgent.exe\"" --autorun"; Flags: uninsdeletevalue

[Run]
; Lancer l'Agent immédiatement après l'installation
Filename: "{app}\\QuietQuestAgent.exe"; Description: "Launch QuietQuest Agent"; Flags: nowait postinstall skipifsilent runascurrentuser

; Fin du script
