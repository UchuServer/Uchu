--------------------------------------------------------------
-- Adds or removes a boost action to the vehicle.
-- created seraas... 2/3/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantom")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
		
		-- we want to scale the amount of backwards impuse we apply based on the mass of the vehicle
		local massMultiplier = vehicle:GetMass().fMass / -3.5
		local linVel = vehicle:GetLinearVelocity()
		local velX = linVel.linVelocity.x * massMultiplier
		local velY = linVel.linVelocity.y * massMultiplier
		local velZ = linVel.linVelocity.z * massMultiplier

		vehicle:ApplyLinearImpulse{linImpulse={x=velX, y=velY, z=velZ}}	
	    
		local addAction = vehicle:VehicleAddSlowdownAction{ fTopSpeedMultiplier = 0.8 }
		
		-- we can assume that our volume is a convex shape and therefore the vehicle can only collide 
		-- with us one time, so their ID will be a valid unique look-up name for this action removal ID
	    self:SetVar(vehicle:GetID(), addAction.iRemoveID)
	end
end


function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantom")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
	    vehicle:VehicleRemoveSlowdownAction{ iRemoveID = self:GetVar(vehicle:GetID()) }
	end
end 