----------------------------------------
-- client side script on Wu to say mission relevent chat depending on where the player is in the mission chain
--
-- created by brandi... 6/14/11
---------------------------------------------

-- last mission in Wu's chain
local FinalMisID = 1964

-- between the first and second mission, say that chat
local middleMisChat = 	{
							{1795,1797,"NINJAGO_WU_CHAT1"},
							{1798,1953,"NINJAGO_WU_CHAT2"},
							{1956,1960,"NINJAGO_WU_CHAT3"},
							{1961,1963,"NINJAGO_WU_CHAT4"}
						}
						
						
function onStartup(self)
	
	-- set proximity on Wu
    self:SetProximityRadius{radius = 30, collisionGroup = 1, name = "conductRadius"}
    
	math.randomseed(os.time())
    
end 


function onProximityUpdate(self,msg)
	-- get the intruder
	local player = msg.objId

	-- if the npc is going to open a mission dialogue, this skips the ambient interact
	if self:GetMissionForPlayer{playerID = player}.missionState ~= 0 then	return end
	
	-- someone entered the vendors bubble
    if msg.status == "ENTER" and msg.name == "conductRadius" then
		
        -- does the intruder really exist?
        if player:Exists() then 
			
			
			-- player has finished the mission chain
			if player:GetMissionCompleteTimestamp{missionID = FinalMisID}.bComplete then

				local num = math.random(5,7)
				self:DisplayChatBubble{wsText = Localize("NINJAGO_WU_CHAT"..num)}
			else
				for k,v in ipairs(middleMisChat) do
					local misState1 = player:GetMissionState{missionID = v[1]}.missionState
					
					-- player has accepted the first mission, but not the second mission
					if misState1 >= 4 then
					
						local misState2 = player:GetMissionState{missionID = v[2]}.missionState
						if misState2 < 2 then
							self:DisplayChatBubble{wsText = Localize(v[3])}
							return
						end
						
					end
					
				end
				
			end
				
			
		end
		
	end
	
end