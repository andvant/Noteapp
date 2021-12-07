!include "MUI2.nsh"

!define NAME "Noteapp"
!define APPFILE "Noteapp.exe"
!define PLATFORM "win64"
!define VERSION "0.1.0"
!define SLUG "${NAME} ${VERSION}"

Name ${NAME}
OutFile "${NAME}-${PLATFORM}-setup.exe"
InstallDir "$PROGRAMFILES64\${NAME}"
; RequestExecutionLevel admin

!define MUI_ICON "res\icon.ico"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "${SLUG} Setup"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "license.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_CHECKED
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop and Start Menu shortcuts"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION finishpageaction

!define MUI_FINISHPAGE_RUN "$INSTDIR\${APPFILE}"
!define MUI_FINISHPAGE_RUN_TEXT "Run ${NAME}"

!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Section
	SetOutPath "$INSTDIR"
	File /r "app\*.*"
	WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

Section "Uninstall"
	Delete "$DESKTOP\${NAME}.lnk"
	RMDir /r "$SMPROGRAMS\${NAME}"
	RMDir /r "$INSTDIR"
SectionEnd

Function finishpageaction
	CreateShortCut "$DESKTOP\${NAME}.lnk" "$INSTDIR\${APPFILE}"
	CreateDirectory "$SMPROGRAMS\${NAME}"
	CreateShortCut "$SMPROGRAMS\${NAME}\${NAME}.lnk" "$INSTDIR\${APPFILE}"
	CreateShortCut "$SMPROGRAMS\${NAME}\uninstall.lnk" "$INSTDIR\uninstall.exe"
FunctionEnd