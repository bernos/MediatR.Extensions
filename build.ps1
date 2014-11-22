Param(
    [Parameter(Position=1,Mandatory=0)]
    [string[]]$task_list = @(),
    
    [Parameter(Position=2,Mandatory=0)]
    [string]$version
)


$configuration = "Release"

$build_file = 'default.ps1'

$projects = @(
    "MediatR.Extensions",
    "MediatR.Extensions.Autofac",
    "MediatR.Extensions.FluentValidation",    
    "MediatR.Extensions.log4net")

$properties = @{
    "configuration" = $configuration;
    "version" = $version;
    "projects" = $projects;
}

import-module .\packages\psake.4.4.1\tools\psake.psm1

invoke-psake $build_file $task_list -Properties $properties