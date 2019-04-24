function onStartup(self)
	print ("Starting up switch")
	self:PlayFXEffect{effectType = "create"}

end

function onCollision(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()

	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		GAMEOBJ:GetZoneControlID():UpdateMissionTask{taskType = "switchon13", target = self}
		--self:PlayFXEffect{effectType = "action"}
	end

	msg.ignoreCollision = true
	return msg
end

function onOffCollision(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()
	print("offcollision generated")

	-- If a player collided with me, then do our stuff
	--if faction and faction.faction == 1 then
		GAMEOBJ:GetZoneControlID():UpdateMissionTask{taskType = "switchoff13", target = self}
		--self:PlayFXEffect{effectType = "action"}
--	end

	msg.ignoreCollision = true
	return msg
end
