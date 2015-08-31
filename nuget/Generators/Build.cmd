%~dp0..\NuGet.exe Update -self
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack EntityLite.nuspec
COPY *.nupkg \\TFS\Nuget /y
pause