--------------------------------------------------------------
-- Adds or removes a boost action to the vehicle.
-- created seraas... 2/3/10
--------------------------------------------------------------


function onCollisionPhantom(self, msg)
	--print("onCollisionPhantomGlobal +.04/.06") 
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
		--print("VehicleAddSlipperyAction")
	    local addAction = msg.objectID:VehicleAddSlipperyAction{fFrontSlipAngleAdd = 0.02, fRearSlipAngleAdd = 0.04}
		--print("RemoveID: " .. addAction.iRemoveID)
		local compoundID = "RemoveID" .. vehicle:GetID()
		--print("Compound ID: " .. compoundID)
	    self:SetVar(compoundID, addAction.iRemoveID)
		--print("SetVar succeeded")
	end
end

function onOffCollisionPhantom(self, msg)
	--print("offCollisionPhantomGlobal")
	local vehicle = msg.objectID
	
	if vehicle:GetID() == GAMEOBJ:GetControlledID():GetID() then
		local compoundID = "RemoveID" .. vehicle:GetID()
		--print("Compound ID: " .. compoundID)
		--print("VehicleRemoveSlipperyAction ID: " .. self:GetVar(compoundID))
	    vehicle:VehicleRemoveSlipperyAction{ iRemoveID = self:GetVar(compoundID) }
		--print("Remove succeeded")
	end
end 