@echo off
echo Running 00045.boolean_short_circuit.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000045.boolean_short_circuit.tlg output.txt
) else (
    TinyLanguage.exe %~dp000045.boolean_short_circuit.tlg %2
)
