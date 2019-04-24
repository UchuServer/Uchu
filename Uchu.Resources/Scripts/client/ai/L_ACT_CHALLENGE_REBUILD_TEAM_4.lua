function onCollision(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		
		local vecString = self:GetVar("bouncer_destination")

		-- Parse the vector3 from the level file into three floats
		local posX = 176
		local posY = 751
		local posZ = -441

		local speed = 95

		-- Create a vector in Lua to pass in message
		local vec = {x = posX, y = posY, z = posZ}
        	
		target:BouncePlayer{niDestPt = vec, fSpeed = speed}
		
		

    	end
	
	msg.ignoreCollision = true
	return msg
end