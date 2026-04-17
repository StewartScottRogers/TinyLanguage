@echo off
echo Running 00090.for_counting_down.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000090.for_counting_down.tlg output.txt
) else (
    TinyLanguage.exe %~dp000090.for_counting_down.tlg %2
)
