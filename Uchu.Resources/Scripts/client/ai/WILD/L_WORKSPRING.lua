function onCollisionPhantom(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()

	if faction and faction.faction == 1 then
        local elast = 2.0
        local maxSpeed = 300.0
		local vec = self:GetUpVector().niUpVector
		local vel = self:GetLinearVelocity().linVelocity
		local playerVel = target:GetLinearVelocity().linVelocity
		target:Deflect{direction = vec, velocity = vel, elasticity = elast, maxSpeed = maximumSpeed, playerVelocity = playerVel}
    end

	return msg

end
