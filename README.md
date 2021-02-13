# PowerShellRPC

A PowerShell module for Discord rich presence.

## Installation
This package is currently not on the PowerShell gallery, so you you'll have to compile it yourself.

Put this into your `$PROFILE` to initialize the rich presence for every PowerShell session and a timer to update the presence:

```powershell
function Start-RichPresence {
    Import-Module "PATH TO THE COMPILED MODULE"
    # connects to RPC
    $null = Start-DRPC
    # updates RPC for the first time
    Update-DRPC
    # logic for the timer which updates the presence every 15 seconds
    $global:DRPCLastUpdate = [System.DateTimeOffset]::Now.Ticks
    $DRPCTimer = New-Object Timers.Timer
    Register-ObjectEvent -InputObject $DRPCTimer -EventName Elapsed -Action { DRPCAction } | Out-Null
    $DRPCTimer.Interval = 15000
    $DRPCTimer.AutoReset = $true
    $DRPCTimer.Start()
    function global:DRPCAction {
        $TPS = 10000000
        if ( ([System.DateTimeOffset]::Now.Ticks - $global:DRPCLastUpdate) -gt (15 * $TPS)) {
            $global:DRPCLastUpdate = [System.DateTimeOffset]::Now.Ticks
            Update-DRPC
        }
    }
}
Start-RichPresence
```

The `Start-RichPresence` cmdlet at the bottom automatically starts the rich presence, you can remove it so you can initialize it yourself.

## Customization

You can use the following cmdlets to specify the `LargeImageKey` and `LargeImageText` that will be used in the presence.

```powershell
# The discord application ID to use. Default is "784135633950081095".
# If the rich presence has already been started use the Stop-DRPC cmdlet to stop it and Start-DRPC to start it back for ApplicationId change to apply
# or you can also just set it before executing the Start-DRPC cmdlet.
[PowerShellRPC.Statics]::ApplicationId = "784135633950081095"
# Set to null by default, if null it is substituted by the name of the host and it's version e.g. "ConsoleHost 7.1.2"
[PowerShellRPC.Statics]::LargeImageText = "PoggersShell!"
# set to "pwsh7" by default, the only other rpc asset on the default application is the windows powershell logo under "pwsh2"
[PowerShellRPC.Statics]::LargeImageKey = "pwsh7"

[PowerShellRPC.Statics]::SmallImageText = "No."
[PowerShellRPC.Statics]::SmallImageKey = "pwsh2"
```

