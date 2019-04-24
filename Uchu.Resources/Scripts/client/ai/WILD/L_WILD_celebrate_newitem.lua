local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onStartup(self,msg)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "StartFlash", self )--start white flash at this time
    --self:PlayAnimation{ animationID = "play" }
end

function onTimerDone(self, msg)

    if (msg.name == "StartFlash") then
        --print "Start fade"
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.00, 0.2, true )--fades into white
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EndFlash", self )
    elseif (msg.name == "EndFlash") then  
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.5, true )--fades from white to normal
    end

end

--/reloadscripts
--/runscript testandexample/testcelebrate