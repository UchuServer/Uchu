function onCollision(self, msg)
	local target = msg.objectID
    	
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and faction.faction == 1 then
		GAMEOBJ:GetZoneControlID():UpdateMissionTask{taskType = "Win", value = GAMEOBJ:GetSystemTime(), target = self}
    	end
	
	msg.ignoreCollision = true
	return msg
end