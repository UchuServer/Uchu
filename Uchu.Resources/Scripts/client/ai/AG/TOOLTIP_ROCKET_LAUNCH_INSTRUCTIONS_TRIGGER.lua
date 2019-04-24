--------------------------------------------------------------
-- Script to display messages to the player when they walk up 
-- to a launch pad in the spaceship.
-- 
-- updated mrb... 4/27/10 Added talk to bob tooltip
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
    local playerID = GAMEOBJ:GetControlledID()
	local targetID = msg.objectID
        
    if msg.objectID:GetID() == playerID:GetID() then    
        local tipText = Localize("ROCKET_DROP_ROCKET_TO_LAUNCH")
        local preCheck1 = playerID:CheckPrecondition{PreconditionID = 8} -- talk to bob
        local preCheck2 = playerID:CheckPrecondition{PreconditionID = 6} -- talk to skylane
        local preCheck3 = playerID:CheckPrecondition{PreconditionID = 107} -- build a rocket
        
        if not preCheck1.bPass then
            tipText = preCheck1.FailedReason
    	elseif not preCheck2.bPass then
            tipText = preCheck2.FailedReason
    	elseif not preCheck3.bPass and not playerID:GetFlag{iFlagID = 54}.bFlag then
            tipText = preCheck2.FailedReason
    	end
    	
        playerID:DisplayTooltip {bShow=true, strText = tipText, iTime = 2500 }
	end
end
