function onStartup(self)
	GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "setvelocity", self )
	self:SetUpdatable{bUpdatable = true}
end

function onTimerDone(self, msg)
	if "setvelocity" == msg.name then
		local rotation = {x = self:GetVar("rotX"), y = self:GetVar("rotY"), z = self:GetVar("rotZ")}
		self:SetAngularVelocity{angVelocity = rotation, bIgnoreDirtyFlags = false}
	end
end