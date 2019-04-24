
--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
	
    if (msg.name == "PauseTime") then
        
        --print ("Timer is done!")
	    
        -- Get the player again (local variables aren't shared by different functions) and return player control
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = false}                                        

    end
	
end

