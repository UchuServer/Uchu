-- This script forwards object drop messages to the parent modular build so you can drag parts into place from your inventory

function onUseItemOnClient(self, msg)

	self:GetParentObj().objIDParent:ClientUseModuleOn{ playerID = msg.playerID, dropTarget = self,
		droppedObjectID = msg.itemToUse, droppedLOT = msg.itemLOT }

end
