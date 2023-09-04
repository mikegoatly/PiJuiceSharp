param(
    [string]$ServerName
)

dotnet publish .\PiJuiceTest.csproj -r "linux-arm" -c Debug -f net7.0 --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -o "publish"

scp -r publish\* "$($ServerName):"

ssh $ServerName 'chmod +x ~/PiJuiceTest'