-- Helper function that disables all of the CLUT timers and resets all effects back to a neutral state
function DoDisableCLUTLightning(self)    
    GAMEOBJ:GetTimer():CancelAllTimers(self)
end

-- We start our effect when the object loads and end it after a certain amount of time
function onStartup(self, msg)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.5, "LittleFlashes", self )    
    GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "StartBigFlash", self ) 
    GAMEOBJ:GetTimer():AddTimerWithCancel( 8.6, "PortalCloseFlash", self )  
end

-- We also disable the effect if the script component is shut down for any reason
function onShutdown(self)    
    GAMEOBJ:GetTimer():CancelAllTimers(self)
end

-- Timers control every aspect of the lightning; this is where the effect itself lives
function onTimerDone(self, msg)
    
    if (msg.name == "LittleFlashes") then
        UI:SendMessage("StartGarmadonFlashes", {})
    end

    if (msg.name == "KillFlashes") then
        UI:SendMessage("KillGarmadonFlashes", {})
    end  
    
    if (msg.name == "PortalCloseFlash") then
        UI:SendMessage("PortalCloseFlash", {})
    end 
    
    if (msg.name == "StartBigFlash") then
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.3, false )  
        GAMEOBJ:GetTimer():AddTimerWithCancel( .3, "EndBigFlash", self )
    end  
    if (msg.name == "EndBigFlash") then
            LEVEL:FadeEffect( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 0.5, false ) 
    end      
end