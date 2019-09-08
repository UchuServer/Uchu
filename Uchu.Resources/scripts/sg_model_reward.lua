require('o_mis')

function onStartup(self)
	self:SetVar("currentModel", -1)
	self:SetVar("childObject", "0")
end

function onSetModelToBuild(self, msg)
	if self:GetVar("currentModel") ~= -1 and msg.templateID ~= -1 then
		local myPos = self:GetPosition{}.pos
		local myRot = self:GetRotation{}
		RESMGR:LoadObject{objectTemplate = self:GetVar("currentModel"),
                          x = myPos.x,
                          y = myPos.y,
                          z = myPos.z,
                          rw = myRot.w,
                          rx = myRot.x,
                          ry = myRot.y,
                          rz = myRot.z,
                          owner = self}
	end
	self:SetVar("currentModel", msg.templateID)
end

function onChildRenderComponentReady(self, msg)
	removeSlamObject(self)
	storeObjectByName(self, "childObject", msg.childID)
	msg.childID:PlayFXEffect{effectType = "slamshake",
							 priority = 0.4,
							 effectID = 595}
	GAMEOBJ:GetTimer():AddTimerWithCancel(  1.0 , "slamtimer", self )
end

function onTimerDone(self, msg)
	if msg.name == "slamtimer" then
		getObjectByName(self, "childObject"):PlayFXEffect{	effectType = "flyto",
															priority = 0.4,
															effectID = 595}
		local chestObjects = self:GetObjectsInGroup{group = "ChestGroup", ignoreSpawners = true}.objects
		if #chestObjects > 0 then
			for index, chest in pairs(chestObjects) do
				chest:PlayAnimation{animationID = "open"}
			end
		else
			print("ERROR: Failed to find chest object.")
		end
		GAMEOBJ:GetTimer():AddTimerWithCancel(  0.75 , "flytimer", self )
	elseif msg.name == "flytimer" then
		removeSlamObject(self)
	end
end

function removeSlamObject(self)
	if self:GetVar("childObject") ~= "0" then
		GAMEOBJ:DeleteObject(getObjectByName(self, "childObject"))
		self:SetVar("childObject", "0")
	end
end
