@echo off
echo Running 00300.caesar_cipher_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000300.caesar_cipher_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000300.caesar_cipher_simple.tlg %2
)
