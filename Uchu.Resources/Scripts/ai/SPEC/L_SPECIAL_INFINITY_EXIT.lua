function onCollision(self, msg)
	local target = msg.objectID

	local faction = target:GetFaction()
	
	if faction and faction.faction == 1 then
        self:PlayFXEffect{effectType = "pickup"}
	end       	

	msg.ignoreCollision = true

  return msg
end