function onCollision(self, msg)
	local target = msg.objectID

	local faction = target:GetFaction()
    
    local position = {}
    position.x = 337 
    position.y = 100
    position.z = -35
    
	if faction and faction.faction == 1 then
        self:PlayFXEffect{effectType = "pickup"}
        target:Teleport{pos=position}
	end       	

	msg.ignoreCollision = true

  return msg
end
