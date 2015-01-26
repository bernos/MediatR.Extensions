Param(
    [Parameter(Position=1,Mandatory=0)]
    [string[]]$task_list = @(),

	[Parameter()]
    [string]$BuildMetaData
)

$build_file = 'default.ps1'

# Properties for the psake build script
$properties = @{

    # Build configuration to use
    "configuration" = "Release";

    # Version number to use if running the Publish build task.
    # This will be read from the command line args
    "version"       = $version;

    # Path to the solution file
    "solution"      = 'MediatR.Extensions.sln';

    # Folder containing source code
    "source_folder" = '';

    # Folder to output deployable packages to. This folder should be ignored
    # from any source control, as we dont commit build artifacts to source
    # control
    "deploy_folder" = 'deploy';

	# Build number metadata that will be appended to semver numers
	"build_meta" = $BuildMetaData;

	
    "projects" = @(
        "MediatR.Extensions",
        "MediatR.Extensions.Autofac",
        "MediatR.Extensions.FluentValidation",    
        "MediatR.Extensions.log4net")

}


import-module .\packages\psake.4.4.1\tools\psake.psm1

invoke-psake $build_file $task_list -Properties $properties