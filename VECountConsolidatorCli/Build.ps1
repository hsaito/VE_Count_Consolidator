$version = "1.7.0.0"
$product = 'VE_Count_Consolidator'

Remove-Item *.zip

$architectures = @("win-x64", "win-x86", "linux-x64", "linux-musl-x64", "linux-arm", "osx-x64")

Foreach ($architecture in $architectures)
{
    dotnet clean
    Remove-Item -LiteralPath bin -Recurse
    Remove-Item -LiteralPath obj -Recurse
    dotnet publish -c Release -r $architecture /p:PublishSingleFile=true
    Compress-Archive ./bin/Release/netcoreapp3.0/$architecture/publish/* "$product-$version-$architecture.zip"
}