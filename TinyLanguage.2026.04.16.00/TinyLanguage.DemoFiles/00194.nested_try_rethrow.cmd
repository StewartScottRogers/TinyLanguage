@echo off
echo Running 00194.nested_try_rethrow.tlg
if "%2"=="" (
    TinyLanguage.exe 00194.nested_try_rethrow.tlg output.txt
) else (
    TinyLanguage.exe 00194.nested_try_rethrow.tlg %2
)
