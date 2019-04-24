function onCollision(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()
	--print("******** Landed ***********")
	local respawnPos = {x=469, y= 265, z = 477}
	local currentPos = target:GetPosition()
	local respawnPoints = self:GetObjectsInGroup{ group = "RespawnPoints" }.objects                   
    local respawnPos = respawnPoints[1]:GetPosition().pos

    target:Teleport{ pos = respawnPos}
    
    
	--end
    	return msg
end
