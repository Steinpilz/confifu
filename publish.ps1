dnu restore src

$projects = @("src\Confifu","src\Confifu.Abstractions")

foreach($project in $projects) {
    Remove-Item .\.published -Force -Recurse
	dnu pack $project --configuration Release --out .published
    $packages = Get-ChildItem .published -Filter *.nupkg -Recurse

    foreach($package in $packages) {
        nuget push $package.FullName $env:nuget_api_key
    }
}
