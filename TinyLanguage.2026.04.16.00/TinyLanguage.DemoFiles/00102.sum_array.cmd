@echo off
echo Running 00102.sum_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00102.sum_array.tlg output.txt
) else (
    TinyLanguage.exe 00102.sum_array.tlg %2
)
