local isBouncing = false

function onCollisionPhantom(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()
	--print("******** Bounce ***********")

	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		if isBouncing == true then
	        return msg
	    else
	        isBouncing = true
	    end
		--local elast = 2.0			-- Will load from level file
		local elast = self:GetVar("springpad_bouncemod")
		if elast == nil then
 			elast = 250
		end
		--print('******************' .. elast)

		--local maximumSpeed = 100.0		-- Will load from level file
		local maximumSpeed = self:GetVar("springpad_maxspeed")
		
		if maximumSpeed == nil then
 			maximumSpeed = 1000
 			--print('******************' .. maximumSpeed)
 		end

		local vec = self:GetUpVector().niUpVector
		local vel = self:GetLinearVelocity().linVelocity	
		local pVel = target:GetLinearVelocity().linVelocity		
		target:Deflect{direction = vec, velocity = vel, playerVelocity = pVel, elasticity = elast, maxSpeed = maximumSpeed}
    end  	
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "BounceCooldown", self )
	return msg
end

onTimerDone = function(self, msg)

    if msg.name == "BounceCooldown" then
     isBouncing = false
    end

end