$script:project_config = "Release"

properties {

  Framework '4.5.1'

  $project_name = "ContosoUniversity"

	if(-not $version)
	{
		$version = if ($env:APPVEYOR_BUILD_VERSION) { $env:APPVEYOR_BUILD_VERSION } else { $defaultVersion }
	}

  $date = Get-Date  
  


  $ReleaseNumber =  $version
  
  Write-Host "**********************************************************************"
  Write-Host "Release Number: $ReleaseNumber"
  Write-Host "**********************************************************************"
  

  $base_dir = resolve-path .
  $build_dir = "$base_dir\build"     
  $source_dir = "$base_dir\src"
  $ui_dir = "$source_dir\$project_name\Content"
  $app_dir = "$source_dir\$project_name"
  $test_dir = "$build_dir\test"
  $result_dir = "$build_dir\results"

  $nuget_exe = "$base_dir\tools\nuget\nuget.exe"

  $roundhouse_dir = "$base_dir\tools\roundhouse"
  $roundhouse_output_dir = "$roundhouse_dir\output"
  $roundhouse_exe_path = "$roundhouse_dir\rh.exe"
  $roundhouse_local_backup_folder = "$base_dir\database_backups"

  $packageId = if ($env:package_id) { $env:package_id } else { "$project_name" }
  $db_server = if ($env:db_server) { $env:db_server } else { ".\sqlexpress" }
  $db_name = if ($env:db_name) { $env:db_name } else { "ContosoUniversity" }
  $test_db_name = if ($env:test_db_name) { $env:test_db_name } else { "$db_name.Tests" }

  $dev_connection_string_name = "$project_name.ConnectionString"
  $test_connection_string_name = "$project_name.Tests.ConnectionString"

  $devConnectionString = if(test-path env:$dev_connection_string_name) { (get-item env:$dev_connection_string_name).Value } else { "Server=$db_server;Database=$db_name;Trusted_Connection=True;MultipleActiveResultSets=true" }
  $testConnectionString = if(test-path env:$test_connection_string_name) { (get-item env:$test_connection_string_name).Value } else { "Server=$db_server;Database=$test_db_name;Trusted_Connection=True;MultipleActiveResultSets=true" }
  
  $db_scripts_dir = "$source_dir\DatabaseMigration"

}
   
#These are aliases for other build tasks. They typically are named after the camelcase letters (rd = Rebuild Databases)
#aliases should be all lowercase, conventionally
#please list all aliases in the help task
task default -depends InitialPrivateBuild
task dev -depends DeveloperBuild
task ci -depends IntegrationBuild
task udb -depends UpdateDatabase
task rdb -depends RebuildAllDatabase
task rdbni -depends RebuildAllDatabaseNoIndexes
task ? -depends help

task help {
   Write-Help-Header
   Write-Help-Section-Header "Comprehensive Building"
   Write-Help-For-Alias "(default)" "Intended for first build or when you want a fresh, clean local copy"
   Write-Help-For-Alias "dev" "Optimized for local dev; Most noteably UPDATES databases instead of REBUILDING"
   Write-Help-For-Alias "ci" "Continuous Integration build (long and thorough) with packaging"
   Write-Help-Section-Header "Database Maintence"
   Write-Help-For-Alias "udb" "Update the Database to the latest version (leave db up to date with migration scripts)"
   Write-Help-For-Alias "rbd" "Rebuild all Databases (dev and test) to the latest version from scratch (useful while working on the schema)"
   Write-Help-For-Alias "rbdni" "Rebuild all Databases w/o Indexes (dev and test) to the latest version from scratch"
   Write-Help-Section-Header "Running Tests"
   Write-Help-For-Alias "rat" "Run all tests"
   Write-Help-Section-Header "Development"
   Write-Help-For-Alias "ui" "Starts Mimosa watch"
   Write-Help-For-Alias "api" "Starts iisexpress on port $iis_port"
   Write-Help-Footer
   exit 0
}

#These are the actual build tasks. They should be Pascal case by convention
task InitialPrivateBuild -depends Clean, Compile, RebuildAllDatabase

task DeveloperBuild -depends Clean, SetDebugBuild, Compile, UpdateDatabase, UpdateTestDatabase
task IntegrationBuild -depends Clean, SetDebugBuild, Compile, RebuildDevDatabase, Package

task CompileOnly -depends Clean, SetDebugBuild, Compile

task SetDebugBuild {
    $script:project_config = "Debug"
}

task SetReleaseBuild {
    $script:project_config = "Release"
}

task RebuildAllDatabase -depends RebuildDevDatabase, RebuildTestDatabase

task RebuildAllDatabaseNoIndexes {
  deploy-database "Rebuild" $devConnectionString $db_scripts_dir "DEV" "none"
  deploy-database "Rebuild" $testConnectionString $db_scripts_dir "TEST" "none"
}

task RebuildDevDatabase{
  deploy-database "Rebuild" $devConnectionString $db_scripts_dir "DEV"  
}

task RebuildTestDatabase {
      deploy-database "Rebuild" $testConnectionString $db_scripts_dir "TEST"
}

task UpdateDatabase {
    deploy-database "Update" $devConnectionString $db_scripts_dir "DEV"
}

task UpdateTestDatabase {
    deploy-database "Update" $testConnectionString $db_scripts_dir "TEST"
}

task Compile -depends Clean { 
    exec { & $nuget_exe restore $source_dir\$project_name.sln }
    exec { msbuild.exe /t:build /v:q /p:Configuration=$project_config /p:Platform="Any CPU" /nologo $source_dir\$project_name.sln }
}

task Clean {
    exec { msbuild /t:clean /v:q /p:Configuration=$project_config /p:Platform="Any CPU" $source_dir\$project_name.sln }
}

task Package {
    exec { msbuild.exe /v:q /p:Configuration=$project_config /nologo $source_dir\$project_name.sln /p:RunOctoPack=true /p:OctoPackPackageVersion=$version /p:OctoPackEnforceAddingFiles=true /p:OctoPackPublishPackageToFileShare=$build_dir'\\packages'}
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions added by Headspring for Help Section
# --------------------------------------------------------------------------------------------------------------

function Write-Help-Header($description) {
   Write-Host ""
   Write-Host "********************************" -foregroundcolor DarkGreen -nonewline;
   Write-Host " HELP " -foregroundcolor Green  -nonewline; 
   Write-Host "********************************"  -foregroundcolor DarkGreen
   Write-Host ""
   Write-Host "This build script has the following common build " -nonewline;
   Write-Host "task " -foregroundcolor Green -nonewline;
   Write-Host "aliases set up:"
}

function Write-Help-Footer($description) {
   Write-Host ""
   Write-Host " For a complete list of build tasks, view default.ps1."
   Write-Host ""
   Write-Host "**********************************************************************" -foregroundcolor DarkGreen
}

function Write-Help-Section-Header($description) {
   Write-Host ""
   Write-Host " $description" -foregroundcolor DarkGreen
}

function Write-Help-For-Alias($alias,$description) {
   Write-Host "  > " -nonewline;
   Write-Host "$alias" -foregroundcolor Green -nonewline; 
   Write-Host " = " -nonewline; 
   Write-Host "$description"
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions 
# --------------------------------------------------------------------------------------------------------------
function deploy-database($action, $connectionString, $scripts_dir, $env, $indexes) {
    $roundhouse_version_file = "$source_dir\$project_name\bin\$project_name.dll"

    write-host "roundhouse version file: $roundhouse_version_file"
    write-host "action: $action"
    write-host "connectionString: $connectionString"    
    write-host "scripts_dir: $scripts_dir"
    write-host "env: $env"

    if (!$env) {
        $env = "LOCAL"
        Write-Host "RoundhousE environment variable is not specified... defaulting to 'LOCAL'"
    } else {
        Write-Host "Executing RoundhousE for environment:" $env
    }  
   
    # Run roundhouse commands on $scripts_dir
    if ($action -eq "Update"){
       exec { &$roundhouse_exe_path -cs "$connectionString" --commandtimeout=300 -f $scripts_dir --env $env --silent -o $roundhouse_output_dir --transaction --amg afterMigration }
    }
    if ($action -eq "Rebuild"){
      $indexesFolder = if ($indexes -ne $null) { $indexes } else { "indexes" }
       exec { &$roundhouse_exe_path -cs "$connectionString" --commandtimeout=300 --env $env --silent -drop -o $roundhouse_output_dir }
       exec { &$roundhouse_exe_path -cs "$connectionString" --commandtimeout=300 -f $scripts_dir -env $env -vf $roundhouse_version_file --silent --simple -o $roundhouse_output_dir --transaction --amg afterMigration --indexes $indexesFolder }
    }
}

function global:delete_file($file) {
    if($file) { remove-item $file -force -ErrorAction SilentlyContinue | out-null } 
}