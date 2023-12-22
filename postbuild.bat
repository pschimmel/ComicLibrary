echo off
set arg1=%1
del C:\Users\patri\OneDrive\ComicLibrary\*.* /s /q
xcopy %arg1%\*.* C:\Users\patri\OneDrive\ComicLibrary /e /c /i /y
