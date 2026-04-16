@echo off
echo Running 00289.chunk_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00289.chunk_array.tlg output.txt
) else (
    TinyLanguage.exe 00289.chunk_array.tlg %2
)
