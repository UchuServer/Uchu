--------------------------------------------------------------
-- Adds or removes a boost action to the vehicle.
-- created seraas... 2/3/10
--------------------------------------------------------------

function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	local player = msg.objectID
	
	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print("VehicleAddPassiveBoostAction")
	    msg.objectID:VehicleAddPassiveBoostAction()
	end
end

function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	local player = msg.objectID
	
	if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print("VehicleRemovePassiveBoostAction")
	    msg.objectID:VehicleRemovePassiveBoostAction()
	end
end 