@echo off
echo Running 00156.triangle_numbers2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000156.triangle_numbers2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000156.triangle_numbers2.tlg %2
)
