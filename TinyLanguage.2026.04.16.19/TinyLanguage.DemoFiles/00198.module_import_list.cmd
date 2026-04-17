@echo off
echo Running 00198.module_import_list.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000198.module_import_list.tlg output.txt
) else (
    TinyLanguage.exe %~dp000198.module_import_list.tlg %2
)
