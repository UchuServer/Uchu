function onStartup(self)

	self:SetProximityRadius { radius = 40, name = "far", iconID = 1 }
	self:SetProximityRadius { radius = 30, name = "mid", iconID = 3 }
	self:SetProximityRadius { radius = 20, name = "near", iconID = 4 }

end

function onGetInteractionDetails(self, msg)

	--msg.IconID = 3417
	msg.TextDetails = "Drop your car here, or be punished."

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	player:DisplayTooltip{bShow=true, strText=""}
	
	return msg

end

-- This function is called when the object starts up or someone requests a pick type update
-- Handling this to set pick type on an object, which makes it able to be interactive
function onGetPriorityPickListType(self, msg)  

    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        
        msg.fCurrentPickTypePriority = myPriority 
 
        msg.ePickType = 14    -- Interactive pick type (Setting to -1 makes something non-interactive) 

    end  
  
    return msg      
end 