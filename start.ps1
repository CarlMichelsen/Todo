$currentDir = Get-Location

# Stop and remove existing jobs
Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Stop-Job
Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Remove-Job

# Kill any dotnet processes running these specific projects
Get-Process dotnet -ErrorAction SilentlyContinue | ForEach-Object {
    $processId = $_.Id
    $commandLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $processId").CommandLine
    if ($commandLine -like "*Identity/App/App.csproj*" -or $commandLine -like "*./App/App.csproj*") {
        Stop-Process -Id $processId -Force
        Write-Host "Killed dotnet process: $processId"
    }
}

# Register Ctrl+C handler
$null = Register-EngineEvent -SourceIdentifier PowerShell.Exiting -Action {
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Stop-Job
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Remove-Job

    Get-Process dotnet -ErrorAction SilentlyContinue | ForEach-Object {
        $processId = $_.Id
        $commandLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $processId").CommandLine
        if ($commandLine -like "*Identity/App/App.csproj*" -or $commandLine -like "*./App/App.csproj*") {
            Stop-Process -Id $processId -Force
        }
    }
}

# Start the jobs
Start-Job -Name "IdentityApp" -ScriptBlock {
    Set-Location $using:currentDir
    dotnet run --project ../Identity/App/App.csproj
}

Start-Job -Name "MainApp" -ScriptBlock {
    Set-Location $using:currentDir
    dotnet run --project ./App/App.csproj
}

try {
    # View output from both jobs
    Get-Job | Receive-Job -Wait -AutoRemoveJob
}
finally {
    # Clean up on exit or Ctrl+C
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Stop-Job
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Remove-Job

    Get-Process dotnet -ErrorAction SilentlyContinue | ForEach-Object {
        $processId = $_.Id
        $commandLine = (Get-CimInstance Win32_Process -Filter "ProcessId = $processId").CommandLine
        if ($commandLine -like "*Identity/App/App.csproj*" -or $commandLine -like "*./App/App.csproj*") {
            Stop-Process -Id $processId -Force
        }
    }
}