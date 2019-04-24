function onCollisionPhantom(self, msg)

	local player = msg.objectID --find players object ID
	
	if not player:Exists() then   --Checking to see if the player still exists
		return 
	end
	local MissionNumber = self:GetVar('mission')
	local Token = self:GetVar('token')
	
	if player:GetMissionState{missionID = MissionNumber}.missionState == 2 then
	
		player:AddItemToInventory{iObjTemplate = Token, itemCount = 1, bMailItemsIfInvFull = true}
	
	end
end
	