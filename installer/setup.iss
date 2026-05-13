; PowerLauncher Inno Setup Script
; Requires Inno Setup 6.x with Dark Theme

#define MyAppName "PowerLauncher"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "PowerLauncher Team"
#define MyAppURL "https://github.com/powerlauncher"
#define MyAppExeName "MinecraftLauncher.exe"
#define PublishDir "..\MinecraftLauncher\bin\Release\net8.0-windows\win-x64\publish"

[Setup]
AppId={{B8F2A1C3-D4E5-6789-ABCD-EF0123456789}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\LICENSE.txt
OutputDir=output
OutputBaseFilename=PowerLauncher-Setup-{#MyAppVersion}
SetupIconFile=..\launcher.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardSizePercent=110,110
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}
DisableProgramGroupPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch PowerLauncher"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\PowerLauncher"

[Code]
const
  DarkBgColor = $1E1E2E;
  DarkFontColor = $F0F0F0;

procedure InitializeWizard();
begin
  WizardForm.Color := DarkBgColor;
  WizardForm.MainPanel.Color := DarkBgColor;
  WizardForm.InnerPage.Color := DarkBgColor;

  WizardForm.PageNameLabel.Font.Color := DarkFontColor;
  WizardForm.PageDescriptionLabel.Font.Color := DarkFontColor;
  WizardForm.WelcomeLabel1.Font.Color := DarkFontColor;
  WizardForm.WelcomeLabel2.Font.Color := DarkFontColor;
  WizardForm.FinishedHeadingLabel.Font.Color := DarkFontColor;
  WizardForm.FinishedLabel.Font.Color := DarkFontColor;

  WizardForm.DirEdit.Color := $2D2D3D;
  WizardForm.DirEdit.Font.Color := DarkFontColor;
  WizardForm.GroupEdit.Color := $2D2D3D;
  WizardForm.GroupEdit.Font.Color := DarkFontColor;

  WizardForm.TasksList.Color := $2D2D3D;
  WizardForm.TasksList.Font.Color := DarkFontColor;

  WizardForm.LicenseMemo.Color := $2D2D3D;
  WizardForm.LicenseMemo.Font.Color := DarkFontColor;

  WizardForm.ComponentsList.Color := $2D2D3D;
  WizardForm.ComponentsList.Font.Color := DarkFontColor;

  WizardForm.ReadyMemo.Color := $2D2D3D;
  WizardForm.ReadyMemo.Font.Color := DarkFontColor;
end;
