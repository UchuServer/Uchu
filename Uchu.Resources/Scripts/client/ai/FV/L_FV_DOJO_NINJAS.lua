--------------------------------------------------------------
-- script on the dojo master to turn on the icons of the other ninjas in the tree when you take his mission
-- 
-- updated Brandi... 3/19/10
--------------------------------------------------------------
function onMissionDialogueOK(self, msg)

    if msg.missionID == 496 and msg.iMissionState <= 2 then 
    
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())   
		local DojoNPCs = self:GetObjectsInGroup{ group = 'DojoNinjas', ignoreSpawners = true }.objects
		
		for k,v in ipairs(DojoNPCs) do 
		
			--print('fireEventt to ' .. v:GetName().name)
			v:FireEvent{senderID = self, args = 'showIcon'}
			
		end
		
	end
	
end