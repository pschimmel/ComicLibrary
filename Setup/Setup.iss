#define ApplicationName 'Comic Library'
#define ApplicationVersion '1.3.0'
#define ApplicationCompany 'Engineering Solutions'

[Setup]
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
AppVerName={cm:NameAndVersion,{#ApplicationName},{#ApplicationVersion}}
VersionInfoVersion={#ApplicationVersion}
VersionInfoProductVersion={#ApplicationVersion}
VersionInfoProductTextVersion={#ApplicationName} {#ApplicationVersion}
VersionInfoCompany={#ApplicationCompany}
VersionInfoCopyright=Copyright 2024 by {#ApplicationCompany}
VersionInfoProductName={#ApplicationName}
AppPublisher={#ApplicationCompany}
AppPublisherURL=http://engineeringsolutions.de
AppSupportURL=http://engineeringsolutions.de
AppUpdatesURL=http://engineeringsolutions.de
AppCopyright=© 2024 by {#ApplicationCompany}
DefaultDirName={autopf}\{#ApplicationCompany}\{#ApplicationName}
DefaultGroupName={#ApplicationCompany}\{#ApplicationName}
SolidCompression=true
UsedUserAreasWarning=no
Compression=lzma2/Ultra64
InternalCompressLevel=ultra64
OutputDir=Output
OutputBaseFilename=Setup {#ApplicationName} {#ApplicationVersion}
WizardImageFile=NormalImage.bmp
WizardSmallImageFile=SmallImage.bmp
ShowLanguageDialog=auto
WizardImageStretch=false
AppId={{F887BBA2-F36D-4477-9666-18B7A74B803C}
UninstallLogMode=overwrite
SetupIconFile=..\Files\Icon Large.ico
UninstallDisplayIcon={app}\ComicLibrary.exe
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True
AppendDefaultDirName=False
RestartApplications=False
DisableWelcomePage=False
ShowTasksTreeLines=True
WizardStyle=modern

[Files]
Source: "..\ComicLibrary\bin\Release\net8.0-windows\*.exe"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\ComicLibrary\bin\Release\net8.0-windows\*.dll"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\ComicLibrary\bin\Release\net8.0-windows\de\*.dll"; DestDir: "{app}\de"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\ComicLibrary\bin\Release\net8.0-windows\*.json"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\ComicLibrary\bin\Release\net8.0-windows\*.config"; DestDir: "{app}"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\ComicLibrary\bin\Release\net8.0-windows\Grading Scales\*.*"; DestDir: "{app}\Grading Scales"; Flags: IgnoreVersion sortfilesbyextension replacesameversion; 
Source: "..\License.txt"; DestDir: "{app}"; Flags: IgnoreVersion replacesameversion; 

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; 

[Icons]
Name: "{group}\Comic Library"; Filename: "{app}\ComicLibrary.exe"; WorkingDir: "{app}"; IconFilename: "{app}\ComicLibrary.exe"; IconIndex: 0; 
Name: "{group}\{cm:UninstallProgram, {#ApplicationName}}"; Filename: "{uninstallexe}"; Flags: preventpinning
Name: "{group}\{cm:ProgramOnTheWeb, {#ApplicationCompany}}"; Filename: "https://github.com/pschimmel/ComicLibrary/"

[Run]
Filename: "{app}\ComicLibrary.exe"; Flags: nowait postinstall skipifdoesntexist; Description: "{cm:LaunchProgram, {#ApplicationName}}"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"; LicenseFile: "..\License.txt"; 
Name: "french"; MessagesFile: "compiler:Languages\French.isl"; LicenseFile: "..\License.txt"; 
Name: "german"; MessagesFile: "compiler:Languages\German.isl"; LicenseFile: "..\License.txt"; 
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"; LicenseFile: "..\License.txt";
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"; LicenseFile: "..\License.txt"; 

[ThirdParty]
UseRelativePaths=True
