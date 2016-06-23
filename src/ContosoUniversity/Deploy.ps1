# These variables should be set via the Octopus web portal:
#
#	RoundhousE.ENV - The RoundhousE variable for running environment-specific scripts. Values are LOCAL, TEST, and DATATEST.
#	SqlServerInstance - "." or ".\SqlExpress", since the Octopus agent has an instance named "."
#

$roundhouse_version_file = ".\bin\ContosoUniversity.dll"
$roundhouse_exe_path = ".\App_Data\migrations\rh.exe"
$scripts_dir = ".\App_Data\migrations\"
$roundhouse_output_dir = ".\App_Data\migrations\output"

if ($OctopusParameters) {
	$env = $OctopusParameters["RoundhousE.ENV"]
	$conn_string = $OctopusParameters["ConnectionString"]
} else {
	$env="LOCAL"
}

Write-Host "RoundhousE is going to run on database: " $conn_string

Write-Host "Executing RoundhousE for environment:" $env

&$roundhouse_exe_path -c "$conn_string" -f $scripts_dir --env $env --silent -o $roundhouse_output_dir
