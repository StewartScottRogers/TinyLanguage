@echo off
echo Running 00193.scope_update_outer.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000193.scope_update_outer.tlg output.txt
) else (
    TinyLanguage.exe %~dp000193.scope_update_outer.tlg %2
)
