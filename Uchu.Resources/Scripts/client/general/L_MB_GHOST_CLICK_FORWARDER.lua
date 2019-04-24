-- This script is used by the modular build "ghost" pieces, to make them start modular build when they're clicked
-- It also forwards object drop messages to the parent modular build so you can drag parts into place from your inventory

local myPriority = 0.8

function onClientUse(self, msg)
	
	msg.targetObject = self:GetParentObj().objIDParent
	
	return msg

end

function onUseItemOnClient(self, msg)

	self:GetParentObj().objIDParent:ClientUseModuleOn{ playerID = msg.playerID, dropTarget = self,
		droppedObjectID = msg.itemToUse, droppedLOT = msg.itemLOT }

end

function onGetPriorityPickListType(self, msg)

    if (self:GetVar("isPickable") ) then

        if ( myPriority > msg.fCurrentPickTypePriority ) then

           msg.fCurrentPickTypePriority = myPriority
           msg.ePickType = 8    -- Build pick type

        end

    end

    return msg

end