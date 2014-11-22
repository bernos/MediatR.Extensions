Properties {
    $version = $null
    $projects = $null
    $configuration = "Release"
}

Task Default -Depends Build

Task Publish -Depends Package {
    foreach($project in $projects) {
        Get-ChildItem | Where-Object -FilterScript {
            ($_.Name.Contains("$project.$version")) -and !($_.Name.Contains(".symbols")) -and ($_.Extension -eq '.nupkg')    
        } | ForEach-Object {
            exec { nuget push $_.FullName }
        }
    }
}

Task Package -Depends Set-Versions,Build {
    foreach($project in $projects) {
        Get-ChildItem -Path "$project\*.csproj" | ForEach-Object {            
            exec { nuget pack -sym $_.FullName -Prop Configuration=$configuration }
        }        
    }
}

Task Build -Depends Clean {
    Exec { msbuild "$Solution" /t:Build /p:Configuration=$configuration } 
}

Task Clean {
    Exec { msbuild "$Solution" /t:Clean /p:Configuration=$configuration } 
}

Task Set-Versions {
    if ($version) {        
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

    } else {
        throw "Please specify a version number."
    }    
}