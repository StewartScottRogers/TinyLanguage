@echo off
echo Running 00214.foreach_nested_loops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000214.foreach_nested_loops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000214.foreach_nested_loops.tlg %2
)
