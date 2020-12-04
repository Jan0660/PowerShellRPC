# PowerShellRPC

A PowerShell module for Discord rich presence.

## Installation
This package is currently not on the 
PowerShell gallery, so you you'll have to compile 
it yourself.

Put this into your `$PROFILE` to start the 
module and a timer to update the presence

```powershell
Import-Module 'PATH TO THE COMPILED .dll'

$null = Start-DRPC
Update-DRPC
$global:DRPCLastUpdate = [System.DateTimeOffset]::Now.Ticks

$Action = { DRPCAction }
    
$DRPCTimer = New-Object Timers.Timer
$EventJob = Register-ObjectEvent -InputObject $DRPCTimer -EventName Elapsed -Action $Action

$DRPCTimer.Interval = 15000
$DRPCTimer.AutoReset = $true
$DRPCTimer.Start()
    
function global:DRPCAction {
    $TPS = 10000000
    if ( ([System.DateTimeOffset]::Now.Ticks - $global:DRPCLastUpdate) -gt (15 * $TPS)){
        $global:DRPCLastUpdate = [System.DateTimeOffset]::Now.Ticks
        Update-DRPC
    }
}
```