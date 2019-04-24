
--require this script to gain access to this function that allows bouncer behavior
function bounceObj(self, target)
	local vecString = self:GetVar("bouncer_destination")

	-- Parse the vector3 from the level file into three floats
	local posX, posY, posZ = string.match(vecString, "(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)\031(%p?%w+%p?%w+)")
	--UI:SendChat{ChatString = vecString, ChatType = "LOCAL", Timestamp = 500}

	local speed = self:GetVar("bouncer_speed")

	-- Create a vector in Lua to pass in message
	local vec = {x = posX, y = posY, z = posZ}

	self:PlayFXEffect{effectType = "onbounce"}
	target:PlayFXEffect{effectType = "onbounce", priority = 1.1}
	target:BouncePlayer{niDestPt = vec, fSpeed = speed, ObjIDBouncer = self}

	-- If player has used a bouncer, and they have a pet, notify the pet about the bounce.
	if target:BelongsToFaction{factionID = 1}.bIsInFaction then
		local pet = target:GetPetID().objID

		if ( pet and pet:Exists() ) then
			-- Let pet know player has used a bouncer
			pet:NotifyPet{ ObjIDSource = target, ObjToNotifyPetAbout = self, iPetNotificationType = 3}
		end
	end
end