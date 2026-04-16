@echo off
echo Running 00278.event_simulation.tlg
if "%2"=="" (
    TinyLanguage.exe 00278.event_simulation.tlg output.txt
) else (
    TinyLanguage.exe 00278.event_simulation.tlg %2
)
