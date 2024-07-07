dotnet build %~dp0..\..\EntityLite.RavenDbProfiler\EntityLite.RavenDbProfiler.csproj -c Release -p:ContinuousIntegrationBuild=true 
%~dp0..\NuGet.exe Update -self
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack EntityLite.RavenDbProfiler.nuspec

IF NOT EXIST C:\LocalPackages\ (mkdir C:\LocalPackages)
COPY %~dp0*.nupkg C:\LocalPackages\ /y
pause