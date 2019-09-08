require('State')
require('o_mis')

SKUNKFOLLOW = {}

 --###########################################################
 --########## Startup
 --###########################################################

function onStartup(self)

	registerWithZoneControlObject(self)

	self:SetProximityRadius { radius = 25 }
	self:SetVar("MaxSkunkFollow", 4)	
	
end


 --###########################################################
 --########## Proximity
 --###########################################################
 
function onProximityUpdate(self, msg)
	if msg.status == "ENTER" then
		print("Mother proximity triggered")
		-- get the local character
        local player = msg.objId
        
        -- only follow players
        if(player:IsCharacter().isChar == true) then
			local MissionState = player:GetMissionState{missionID = 133}.missionState
			if(MissionState == 2) then 
			
				if (SKUNKFOLLOW[player:GetID()] ~= nil) then
					if(SKUNKFOLLOW[player:GetID()]["num"] >= self:GetVar("MaxSkunkFollow")) then
						print("Completing mother skunk mission for player")
						player:UpdateMissionTask{target = self, value = 133, taskType= "complete" }
						
						for skunkNum = 1, SKUNKFOLLOW[player:GetID()]["num"] do
							if(SKUNKFOLLOW[player:GetID()][skunkNum] ~= nil) then
								local skunk = GAMEOBJ:GetObjectByID(SKUNKFOLLOW[player:GetID()][skunkNum])
								if(skunk ~= nil) then
									skunk:Die{killType = "SILENT"}
								end
							end
						end
			
						SKUNKFOLLOW[player:GetID()] = nil
					else
						-- tooltip? to get max?
						-- self:Help{rerouteID = user, iHelpID = CONSTANTS["CLIENT_TOOLTIP_MISSION_ACCEPT"]}
						print("NOT ENOUGH SKUNKS")
					end
					
				else
					print("player has no skunks")
				end
				
			end
			
		end
		
	end
   
end


 --###########################################################
 --########## Spawn baby skunks
 --###########################################################

function SpawnBabySkunks(self)

end


 --###########################################################
 --########## Baby Skunk Loaded
 --###########################################################
 
onChildLoaded = function(self,msg)

end


--###########################################################
--########## Messages from Baby Skunks (awwwwwww)
--###########################################################
function onNotifyObject(self, msg)
    
end


--###########################################################
--########## Ontimer done
--###########################################################
 
onTimerDone = function(self, msg)

end



function onUse(self, msg)
	
   
end



-- Request follow message from baby skunks
function onRequestFollow(self, msg)
    
    print("IN ON MOTHER SKUNK REQUESTFOLLOW")
    -- Check to see if user has any skunks following
    
    -- See if this user has an entry
    if (SKUNKFOLLOW[msg.targetID:GetID()] ~= nil) then
		print("ENTRY FOUND")
		if(SKUNKFOLLOW[msg.targetID:GetID()]["num"] >= self:GetVar("MaxSkunkFollow")) then
			print("MAX SKUNKS")
			msg.bCanFollow = false
		else
			msg.bCanFollow = true
			msg.iPosit = SKUNKFOLLOW[msg.targetID:GetID()]["num"] + 1
			SKUNKFOLLOW[msg.targetID:GetID()]["num"] = msg.iPosit
			SKUNKFOLLOW[msg.targetID:GetID()][msg.iPosit] = msg.requestorID:GetID()
			
			if(SKUNKFOLLOW[msg.targetID:GetID()]["num"] >= self:GetVar("MaxSkunkFollow")) then
				if(msg.targetID ~= nil) then
					msg.targetID:DisplayTooltip{ bShow = true, strText = "Return to the Mother Skunk. You only have 1 minute!", iTime = 0 }
				end
			end
		end
    else
		print("ENTRY NOT FOUND")
		msg.bCanFollow = true
		msg.iPosit = 1
		SKUNKFOLLOW[msg.targetID:GetID()] = {}
		SKUNKFOLLOW[msg.targetID:GetID()]["num"] = 1
		SKUNKFOLLOW[msg.targetID:GetID()][msg.iPosit] = msg.requestorID:GetID()
    end
    
    return msg
	
end
