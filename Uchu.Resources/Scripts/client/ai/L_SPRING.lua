function onCollisionPhantom(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		--local elast = 2.0			-- Will load from level file
		local elast = self:GetVar("springpad_bouncemod")

		--local maximumSpeed = 100.0		-- Will load from level file
		local maximumSpeed = self:GetVar("springpad_maxspeed")
        	
		local vec = self:GetUpVector().niUpVector
		local vel = self:GetLinearVelocity().linVelocity
		local playerVel = target:GetLinearVelocity().linVelocity
		target:Deflect{direction = vec, velocity = vel,	elasticity = elast, maxSpeed = maximumSpeed, playerVelocity = playerVel}
    	end

	return msg
end
        