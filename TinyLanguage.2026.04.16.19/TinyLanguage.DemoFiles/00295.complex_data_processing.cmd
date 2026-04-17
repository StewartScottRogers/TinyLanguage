@echo off
echo Running 00295.complex_data_processing.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000295.complex_data_processing.tlg output.txt
) else (
    TinyLanguage.exe %~dp000295.complex_data_processing.tlg %2
)
