require('o_mis')

function onStartup(self)
	self:SetVar("currentModel", 6783)
	self:SetVar("childObject", "0")
end

function onSetModelToBuild(self, msg)
	print("spider model ???")

	if self:GetVar("currentModel") ~= -1 and msg.templateID ~= -1 then
		local myPos = self:GetPosition{}.pos
		local myRot = self:GetRotation{}
		RESMGR:LoadObject{objectTemplate = msg.templateID),
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

