@echo off
echo Running 00126.for_scope_isolation.tlg
if "%2"=="" (
    TinyLanguage.exe 00126.for_scope_isolation.tlg output.txt
) else (
    TinyLanguage.exe 00126.for_scope_isolation.tlg %2
)
