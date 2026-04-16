@echo off
echo Running 00204.bool_short_circuit.tlg
if "%2"=="" (
    TinyLanguage.exe 00204.bool_short_circuit.tlg output.txt
) else (
    TinyLanguage.exe 00204.bool_short_circuit.tlg %2
)
