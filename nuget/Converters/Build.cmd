%~dp0..\NuGet.exe Update -self
dotnet build %~dp0..\..\inercya.Newtonsoft.Json\inercya.Newtonsoft.Json.Converters.csproj -c Release -p:ContinuousIntegrationBuild=true 
REM %~dp0..\NuGet.exe SetApiKey MyApiKey
%~dp0..\NuGet.exe Pack inercya.Newtonsoft.Json.Converters.nuspec 

IF NOT EXIST C:\LocalPackages\ (mkdir C:\LocalPackages)
COPY %~dp0*.nupkg C:\LocalPackages\ /y
pause
