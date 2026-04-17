@echo off
echo Running 00098.foreach_index_track.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000098.foreach_index_track.tlg output.txt
) else (
    TinyLanguage.exe %~dp000098.foreach_index_track.tlg %2
)
