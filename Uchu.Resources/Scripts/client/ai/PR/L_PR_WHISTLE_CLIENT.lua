--------------------------------------------------------------
-- Client side script on Coalessa in pet rock
-- this script checks the missions state and player flags

-- created by Brandi... 2/17/10
--------------------------------------------------------------

function onMissionDialogueOK(self,msg)
    --print('onMissionDialogueOK ' .. msg.iMissionState .. ' - ' .. msg.missionID)
    
	--checks if mission is tame a pet mission
    if msg.missionID == 110 then
	
		--if the player just accepted the mission, give them the pet taming emote
        if msg.iMissionState < 2 then
			
			--print('unlocking emote')
			--have to send to a server script because the emotes cant be unlocked from the client
			self:FireEventServerSide{senderID = msg.responder, args = 'unlockEmote' }
			
		end
	
	--if the mission is the tame every other pet in the game mission (more to be added to this later)
	elseif msg.missionID == 688 then
		
		--*** once the pets have scripts on them to set the player flags when they are tamed, need to add a check to see
		--	  how many different pets the player has tamed to up date this mission
		
		
		--if the player completed the mission set their flag on 
		if msg.iMissionState == 4 then
			msg.responder:SetFlag{iFlagID = 67, bFlag = true}
			
			
			--wont actually need this in the real game, the object to spawn the lion is in a different map
			local group = self:GetObjectsInGroup{group = "lionstuff", ignoreSpawners = true}.objects
						for k,v in ipairs(group) do
			
				if v then
				
				   v:RequestPickTypeUpdate()
				end
			end
			
			
		end
	end
	
end
