--------------------------------------------------------------
-- Script to unlock minimap after mission is accepted and cinematic is played
-- 
-- updated mrb... 3/26/10
--------------------------------------------------------------

--//////////////////////////////////////////////////////////////////////////////////
local misID = 311           -- missionID from DB
--//////////////////////////////////////////////////////////////////////////////////

function onMissionDialogueOK(self, msg)            
    local player = GAMEOBJ:GetControlledID()
    
    if not player then return end
    
    if msg.missionID == misID and msg.responder:GetID() == player:GetID() then             
		if not ( player:GetFlag{iFlagID = 2}.bFlag ) then			
            local cineTime = LEVEL:GetCinematicInfo("Mission_Wisp_Cam_311") or 1
            
		    GAMEOBJ:GetTimer():AddTimerWithCancel( cineTime + 1, "unlockMinimap",self )
	    end
    end
end


function onTimerDone(self, msg)
    if msg.name == "unlockMinimap" then
        local player = GAMEOBJ:GetControlledID()
        
        if not player then return end
        
        -- 2 is the minimap tutorial
        player:Help{ iHelpID = 2 }        
    end
end 