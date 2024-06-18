dotnet build %~dp0..\..\EntityLite.MassTransit\EntityLite.MassTransit.csproj -c Release -p:ContinuousIntegrationBuild=true 
%~dp0..\NuGet.exe Update -self
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack EntityLite.MassTransit.nuspec

IF NOT EXIST C:\LocalPackages\ (mkdir C:\LocalPackages)
COPY %~dp0*.nupkg C:\LocalPackages\ /y
pause