
$v = $args[0]

Get-ChildItem -Recurse -Force | Where-Object { $_.Name -eq "AssemblyInfo.cs" } | ForEach-Object {
    (Get-Content $_.FullName) | ForEach-Object {
        ($_ -replace 'AssemblyVersion\(.*\)', ('AssemblyVersion("' + $v + '")')) -replace 'AssemblyFileVersion\(.*\)', ('AssemblyFileVersion("' + $v + '")')
    } | Set-Content $_.FullName -Encoding UTF8
}

Get-ChildItem -Recurse -Force | Where-Object { $_.Name -like "*.nuspec" } | ForEach-Object {
	(Get-Content $_.FullName) | ForEach-Object {
		($_ -replace '<dependency id="MediatR.Extensions" version=".*"/>', ('<dependency id="MediatR.Extensions" version="' + $v + '"/>'))
	} | Set-Content $_.FullName -Encoding UTF8
}