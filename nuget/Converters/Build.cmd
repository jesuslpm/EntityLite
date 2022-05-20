%~dp0..\NuGet.exe Update -self
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack inercya.Newtonsoft.Json.Converters.nuspec
REM COPY *.nupkg \\TFS\Nuget /y
pause