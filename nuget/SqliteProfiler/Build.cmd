dotnet build %~dp0..\..\inercya.EntityLite.SqliteProfiler\inercya.EntityLite.SqliteProfiler.csproj -c Release
%~dp0..\NuGet.exe Update -self
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack EntityLite.SqliteProfiler.nuspec

IF NOT EXIST C:\LocalPackages\ (mkdir C:\LocalPackages)
COPY %~dp0*.nupkg C:\LocalPackages\ /y
pause