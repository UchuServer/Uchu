--------------------------------------------------------------
-- client script on the fx assets under the minidragons in the monastery courtyard
-- 
-- created by brandi... 7/26/11
--------------------------------------------------------------

-- daily mission to collect the dragon emblems
local minidragonsMisID = 2040

-- hidden achievement to collect the dragon emblems based on fx obj LOT
local minidragonAchieveIDs = {
								[16547]  	= 2064, -- earth
								[16548]		= 2065, -- ice
								[16549]		= 2066,	-- lightning
								[16550]		= 2067  -- fire
							 }
							 
----------------------------------------------
-- the object ghosted in or out for the player
----------------------------------------------					 
function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetControlledID()
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

----------------------------------------------
-- the zone control object says the player is loaded
----------------------------------------------
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetControlledID()
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

----------------------------------------------
-- decide to turn on the fx or not
----------------------------------------------
function CheckMissions(self,player)

	-- get the state that the player is in on the dragon emblem daily mission
	local miniDragState = player:GetMissionState{missionID = minidragonsMisID}.missionState
	
	-- if the mission is available to the player, turn the fx off
	if miniDragState == 0 or miniDragState == 1 or miniDragState == 9 then 
		TurnOffFx(self)
	
	-- the player is on the mission 
	elseif miniDragState == 2 or miniDragState == 10  then
	
		-- get the lot of this obj
		local myLOT = self:GetLOT().objtemplate
		-- if the player hasnt completed the hidden achievement to get the dragon emblem
		if player:GetMissionState{ missionID = minidragonAchieveIDs[myLOT] }.missionState == 8 then
			TurnOnFx(self)
		else
			TurnOffFx(self)
		end
		
	-- only states left are that the mission is ready to turn in, meaning the player has completed all of the hidden achievements
	-- no need to check them, just turn the fx on
	else
		
		TurnOnFx(self)
		
	end

end

----------------------------------------------
-- turn on the fx 
----------------------------------------------
function TurnOnFx(self)
	self:PlayFXEffect{effectType = "on", name = "on"}
end

----------------------------------------------
-- turn off the fx 
----------------------------------------------
function TurnOffFx(self)
	self:StopFXEffect{name = "on" }
end
	
