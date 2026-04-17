@echo off
echo Running 00284.advanced_recursion_tower.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000284.advanced_recursion_tower.tlg output.txt
) else (
    TinyLanguage.exe %~dp000284.advanced_recursion_tower.tlg %2
)
