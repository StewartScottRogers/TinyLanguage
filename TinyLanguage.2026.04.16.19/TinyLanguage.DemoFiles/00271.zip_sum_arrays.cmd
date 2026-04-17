@echo off
echo Running 00271.zip_sum_arrays.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000271.zip_sum_arrays.tlg output.txt
) else (
    TinyLanguage.exe %~dp000271.zip_sum_arrays.tlg %2
)
