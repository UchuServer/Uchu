--------------------------------------------------------------
-- Adds or removes a boost action to the vehicle.
-- created seraas... 2/3/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantomSecondary +.02/.04") 
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print("VehicleAddSlipperyAction")
	    local addAction = msg.objectID:VehicleAddSlipperyAction{fFrontSlipAngleAdd = 0.01, fRearSlipAngleAdd = 0.02}
		--print("RemoveID: " .. addAction.iRemoveID)
		local compoundID = "RemoveID" .. vehicle:GetID()
		--print("Compound ID: " .. compoundID)
	    self:SetVar(compoundID, addAction.iRemoveID)
		--print("SetVar succeeded")
	end
end

function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantomSecondary")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
		local compoundID = "RemoveID" .. vehicle:GetID()
		--print("Compound ID: " .. compoundID)
		--print("VehicleRemoveSlipperyAction ID: " .. self:GetVar(compoundID))
	    vehicle:VehicleRemoveSlipperyAction{ iRemoveID = self:GetVar(compoundID) }
		--print("Remove succeeded")
	end
end 