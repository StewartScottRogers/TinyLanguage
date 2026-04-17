@echo off
echo Running 00308.is_operator_all_types.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000308.is_operator_all_types.tlg output.txt
) else (
    TinyLanguage.exe %~dp000308.is_operator_all_types.tlg %2
)
