@echo off
echo Running 00166.array_sum_reduce.tlg
if "%2"=="" (
    TinyLanguage.exe 00166.array_sum_reduce.tlg output.txt
) else (
    TinyLanguage.exe 00166.array_sum_reduce.tlg %2
)
