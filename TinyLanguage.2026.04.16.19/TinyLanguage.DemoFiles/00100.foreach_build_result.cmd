@echo off
echo Running 00100.foreach_build_result.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000100.foreach_build_result.tlg output.txt
) else (
    TinyLanguage.exe %~dp000100.foreach_build_result.tlg %2
)
