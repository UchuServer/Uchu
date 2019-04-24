function onCollision(self, msg)
	local target = msg.objectID
    	
	--UI:SendChat{ChatString = "Collision!", ChatType = "LOCAL", Timestamp = 500}
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		
		local vecString = self:GetVar("bouncer_destination")
		local speed = self:GetVar("bouncer_speed")
		
		if vecString and speed then
		-- Parse the vector3 from the level file into three floats
		local posX, posY, posZ = string.match(vecString, "(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)")
		

		-- Create a vector in Lua to pass in message
		local vec = {x = posX, y = posY, z = posZ}
        	
		target:BouncePlayer{niDestPt = vec, fSpeed = speed}
		
		
	end
    	end

	msg.ignoreCollision = true
	return msg
end

--SUPER HACK!! (WE'RE SO AWESOME)
--Should use a message specifically to set bouncer params
function onEmotePlayed(self, msg)
	x, y, z, speed  = string.match(msg.emoteID, "(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)#(%d+)")
	vector = x .. '\031' .. y .. '\031' .. z
	self:SetVar("bouncer_destination", vector)
	self:SetVar("bouncer_speed", speed)
end