function onCollisionPhantom(self, msg)

	local target = msg.objectID
    	
	local elast = self:GetVar("springpad_bouncemod")
    
    self:PlayFXEffect {priority = 1.2, effectType = "pinball_bumper"}

	--target:PlaySound{ strSoundName = "Pinball_Bumper" }
	--msg.objectID:PlaySound{ strSoundName = "Pinball_Bumper3" }

	--local maximumSpeed = 100.0		-- Will load from level file
	local maximumSpeed = self:GetVar("springpad_maxspeed")
    	
	local vec = self:GetUpVector().niUpVector
	local vel = self:GetLinearVelocity().linVelocity
	local playerVel = target:GetLinearVelocity().linVelocity
	target:Deflect{direction = vec, velocity = vel,	elasticity = elast, maxSpeed = maximumSpeed, playerVelocity = playerVel}

	return msg
end
        