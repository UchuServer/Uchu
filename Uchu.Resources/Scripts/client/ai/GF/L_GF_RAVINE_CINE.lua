require('o_mis') --11

function onCollisionPhantom(self, msg) 
    local playerID = GAMEOBJ:GetLocalCharID()
    
    -- exclusion checks        
    if msg.objectID:GetID() ~= playerID then return end
    -- exclusion checks     s
	local player = msg.objectID	
	
    if not player:GetFlag{iFlagID = 36}.bFlag then
	
		player:SetUserCtrlCompPause{bPaused = true}
		player:PlayCinematic { pathName = "Ravine" }
		player:SetFlag{iFlagID = 36, bFlag = true} 
		GAMEOBJ:GetTimer():AddTimerWithCancel( 11, "CameraTimer", self )
	end
end

function onTimerDone (self,msg)
    if (msg.name == "CameraTimer") then      
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
        player:SetUserCtrlCompPause{bPaused = false}
    end
end

