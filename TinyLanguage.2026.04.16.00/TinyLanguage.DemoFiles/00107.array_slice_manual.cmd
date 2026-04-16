@echo off
echo Running 00107.array_slice_manual.tlg
if "%2"=="" (
    TinyLanguage.exe 00107.array_slice_manual.tlg output.txt
) else (
    TinyLanguage.exe 00107.array_slice_manual.tlg %2
)
