$version = "1.7.0.0"
$product = 'VE_Count_Consolidator'
$framework = 'net10.0'

Remove-Item *.zip -ErrorAction SilentlyContinue

$architectures = @("win-x64", "win-x86", "linux-x64", "linux-musl-x64", "linux-arm", "osx-x64")

Foreach ($architecture in $architectures)
{
    dotnet clean
    Remove-Item -LiteralPath bin -Recurse -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath obj -Recurse -ErrorAction SilentlyContinue
    dotnet publish -c Release -r $architecture -f $framework /p:PublishSingleFile=true
    Compress-Archive ./bin/Release/$framework/$architecture/publish/* "$product-$version-$architecture.zip"
}