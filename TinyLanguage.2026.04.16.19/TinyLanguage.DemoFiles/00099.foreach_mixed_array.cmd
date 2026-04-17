@echo off
echo Running 00099.foreach_mixed_array.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000099.foreach_mixed_array.tlg output.txt
) else (
    TinyLanguage.exe %~dp000099.foreach_mixed_array.tlg %2
)
