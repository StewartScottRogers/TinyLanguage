@echo off
echo Running 00201.nested_arrays.tlg
if "%2"=="" (
    TinyLanguage.exe 00201.nested_arrays.tlg output.txt
) else (
    TinyLanguage.exe 00201.nested_arrays.tlg %2
)
