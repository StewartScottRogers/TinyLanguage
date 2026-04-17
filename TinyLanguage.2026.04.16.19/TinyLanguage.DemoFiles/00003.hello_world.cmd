@echo off
echo Running 00003.hello_world.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000003.hello_world.tlg output.txt
) else (
    TinyLanguage.exe %~dp000003.hello_world.tlg %2
)
