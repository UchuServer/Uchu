--------------------------------------------------------------
-- Client side script to play the darkitect reveal
-- 
-- Converted for Nexus Tower by Ray 12/15/10
--------------------------------------------------------------

--------------------------------------------------------------
-- the server script will tell the client script when the player completes the mission
--------------------------------------------------------------
require('L_NPC_WONG_CLIENT')

function onRenderComponentReady(self,msg)
	self:SetOverheadIconOffset{horizOffset = 0, vertOffset = 9, depthOffset = 0}
end

function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    if ( myPriority > msg.fCurrentPickTypePriority ) then

       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type

    end

    return msg
end 
