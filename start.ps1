$currentDir = Get-Location

# Function to clean up jobs and processes quickly
function Stop-AppJobs {
    # Stop jobs forcefully first
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | Stop-Job -PassThru | Remove-Job -Force

    # Kill dotnet processes efficiently (without slow WMI queries)
    Get-Process -Name dotnet -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -and (
        (Get-Process -Id $_.Id -ErrorAction SilentlyContinue).CommandLine -like "*Identity/App/App.csproj*" -or
                (Get-Process -Id $_.Id -ErrorAction SilentlyContinue).CommandLine -like "*./App/App.csproj*"
        )
    } | Stop-Process -Force -ErrorAction SilentlyContinue

    # Alternative: Kill ALL dotnet processes from child processes of the jobs
    Get-Job -Name "IdentityApp", "MainApp" -ErrorAction SilentlyContinue | ForEach-Object {
        if ($_.ChildJobs[0].Output) {
            Get-Process -Id $_.ChildJobs[0].ProcessId -ErrorAction SilentlyContinue |
                    ForEach-Object { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue }
        }
    }
}

# Initial cleanup
Stop-AppJobs

# Start the jobs
Start-Job -Name "IdentityApp" -ScriptBlock {
    Set-Location $using:currentDir
    dotnet run --project ../Identity/App/App.csproj
} | Out-Null

Start-Job -Name "MainApp" -ScriptBlock {
    Set-Location $using:currentDir
    dotnet run --project ./App/App.csproj
} | Out-Null

try {
    # View output from both jobs
    Get-Job | Receive-Job -Wait -AutoRemoveJob
}
finally {
    Stop-AppJobs
}