--------------------------------------------------------------
-- Client Script on the ninjas in FV that were freed from GF
-- this sets interaction text and visible state

-- updated Brandi... 8/2/10 - kept one script on all ninjas, but makes only the ones that have been freed visible or not
-- updated by brandi... 4/14/11 - removed heartbeat timer
--------------------------------------------------------------
require('o_mis')
require('L_AG_NPC')

local freeMis = {LOT_9256 = 701,LOT_9298 = 702,LOT_9299 = 703,LOT_9300 = 704}

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech	
	AddInteraction(self, "interactionText", Localize("FREED_NINJA_CLICK_A")) 
	AddInteraction(self, "interactionText", Localize("FREED_NINJA_CLICK_B")) 
	AddInteraction(self, "interactionText", Localize("FREED_NINJA_CLICK_C")) 
	AddInteraction(self, "interactionText", Localize("FREED_NINJA_CLICK_D")) 
	AddInteraction(self, "interactionText", Localize("FREED_NINJA_CLICK_E")) 
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("FREED_NINJA_RANDOM_A"))  -- Ah, it's good to be home.
	AddInteraction(self, "proximityText", Localize("FREED_NINJA_RANDOM_B"))  --The air is so much fresher here.
	AddInteraction(self, "proximityText", Localize("FREED_NINJA_RANDOM_C"))  --Week after week, nothing but bananas. It's enough to turn your skin yellow.
	AddInteraction(self, "proximityText", Localize("FREED_NINJA_RANDOM_D"))  --Thanks for helping me escape.
	AddInteraction(self, "proximityText", Localize("FREED_NINJA_RANDOM_E"))  --That monkey was working for us, you know.


end

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end


function CheckMissions(self,player)
	-- check to see if the player has taken the mission from Olivia and not healed Rutger net
	if player:GetMissionState{missionID = freeMis["LOT_"..self:GetLOT().objtemplate]}.missionState ~= 8 then
		self:SetVisible{visible = false, fadeTime = 0}
	end
	
end
