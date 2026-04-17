@echo off
echo Running 00167.zip_arrays2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000167.zip_arrays2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000167.zip_arrays2.tlg %2
)
