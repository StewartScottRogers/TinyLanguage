@echo off
echo Running 00020.variable_scope_outer.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000020.variable_scope_outer.tlg output.txt
) else (
    TinyLanguage.exe %~dp000020.variable_scope_outer.tlg %2
)
