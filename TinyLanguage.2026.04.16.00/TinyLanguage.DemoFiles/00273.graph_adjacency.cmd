@echo off
echo Running 00273.graph_adjacency.tlg
if "%2"=="" (
    TinyLanguage.exe 00273.graph_adjacency.tlg output.txt
) else (
    TinyLanguage.exe 00273.graph_adjacency.tlg %2
)
