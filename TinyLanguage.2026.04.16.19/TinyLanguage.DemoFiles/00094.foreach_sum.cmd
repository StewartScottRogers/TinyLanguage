@echo off
echo Running 00094.foreach_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000094.foreach_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000094.foreach_sum.tlg %2
)
