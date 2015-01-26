Properties {
    $projects = $null
    $configuration = "Release"
    $source_folder = $null
    $solution = $null
	$build_meta = $null
}

Task Default -Depends Build

Task Publish -Depends Package {
    $version = getVersionBase
    
    foreach($project in $projects) {
        Get-ChildItem | Where-Object -FilterScript {
            ($_.Name.Contains("$project.$version")) -and !($_.Name.Contains(".symbols")) -and ($_.Extension -eq '.nupkg')    
        } | ForEach-Object {
            exec { nuget push $_.FullName }
        }
    }
}

Task Package -Depends Test {
    foreach($project in $projects) {
        Get-ChildItem -Path "$project\*.csproj" | ForEach-Object {            
            exec { nuget pack -sym $_.FullName -Prop Configuration=$configuration }
        }        
    }
}

Task Test -Depends Build {
    Get-ChildItem $source_folder -Recurse -Include *Tests.csproj | % {
        Exec { & ".\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe" "$($_.DirectoryName)\bin\$configuration\$($_.BaseName).dll" /noshadow }
    }
}

Task Build -Depends Clean,Set-Versions {
    Exec { msbuild "$solution" /t:Build /p:Configuration=$configuration } 
}

Task Clean {
    Exec { msbuild "$solution" /t:Clean /p:Configuration=$configuration } 
}

Task Set-Versions {
    $version = getVersionBase

	if ($build_meta) {
        "##teamcity[buildNumber '$version+$build_meta']" | Write-Host
    } else {
		"##teamcity[buildNumber '$version']" | Write-Host
	}

    Get-ChildItem -Recurse -Force | Where-Object { $_.Name -eq "AssemblyInfo.cs" } | ForEach-Object {
        (Get-Content $_.FullName) | ForEach-Object {
            ($_ -replace 'AssemblyVersion\(.*\)', ('AssemblyVersion("' + $version + '")')) -replace 'AssemblyFileVersion\(.*\)', ('AssemblyFileVersion("' + $version + '")')
        } | Set-Content $_.FullName -Encoding UTF8
    }

    Get-ChildItem -Recurse -Force | Where-Object { $_.Name -like "*.nuspec" } | ForEach-Object {
        (Get-Content $_.FullName) | ForEach-Object {
            ($_ -replace '<dependency id="MediatR.Extensions" version=".*"/>', ('<dependency id="MediatR.Extensions" version="' + $version + '"/>'))
        } | Set-Content $_.FullName -Encoding UTF8
    }
    
}

function getVersionBase {
    $versionInfo = (Get-Content "version.json") -join "`n" | ConvertFrom-Json
    "$($versionInfo.major).$($versionInfo.minor).$($versionInfo.patch)";    
}