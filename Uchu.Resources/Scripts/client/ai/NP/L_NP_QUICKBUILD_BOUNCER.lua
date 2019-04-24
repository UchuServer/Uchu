-- The destination string is seperated by semi-colons so HF can 
-- set config data easily
function onCollision(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		
		local vecString = self:GetVar("bouncer_destination")
		local speed = self:GetVar("bouncer_speed")
		
		if vecString and speed then
			-- Parse the vector3 from the level file into three floats
			local posX, posY, posZ = string.match(vecString, "(%p?%w+%p?%w+);(%p?%w+%p?%w+);(%p?%w+%p?%w+)")
		
			-- Create a vector in Lua to pass in message
			local vec = {x = posX, y = posY, z = posZ}

			self:PlayFXEffect{effectType = "onbounce"}
			target:PlayFXEffect{effectType = "onbounce", priority = 1.1}
			target:BouncePlayer{niDestPt = vec, fSpeed = speed}

		end
    	end

	msg.ignoreCollision = true
	return msg
end
