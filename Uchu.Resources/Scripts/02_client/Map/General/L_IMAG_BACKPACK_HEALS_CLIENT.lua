--------------------------------------------------------------
-- Client side script on npcs that can be healed with the imagination backpack

-- created by brandi.. 2/15/11
-- updated by brandi.. 4/18/11 - the missions are now set on the object so this script can be used on brick fury as well.
--------------------------------------------------------------

-- set config data for the missions in HF. If the same mission turns the fx on as turns the fx off, then you only need FXOnMis
-- in HF   FXOnMis  1:####
--         FXOffMis 1:####

----------------------------------------------
-- When the render on the npc is loaded for the client
----------------------------------------------
function onScopeChanged(self,msg)
	baseScopeChanged(self,msg)
end

function baseScopeChanged(self,msg)
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
	baseNotifyPlayerReady(self,zoneObj,msg)
end

function baseNotifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end


function CheckMissions(self,player)	
	local FXOnCheck = self:GetVar("FXOnMis")
	if not FXOnCheck then return end
	local FXOffCheck = self:GetVar("FXOffMis") or FXOnCheck
	-- check to see if the player is on the mission to heal
	if player:GetMissionState{missionID = FXOnCheck}.missionState >= 2 and 
				player:GetMissionState{missionID = FXOffCheck}.missionState < 4 then
		self:PlayFXEffect{name = "infection", effectType = "visible"}
	end
	
end
----------------------------------------------
-- server side objects sending messages to the client
----------------------------------------------
function onNotifyClientObject(self,msg)
	baseNotifyClientObject(self,msg)
end

function baseNotifyClientObject(self,msg)
	-- if the message is to clearmaelstrom, stop the fx
	if msg.name == "ClearMaelstrom" then
		self:StopFXEffect{name = "infection"}
	-- if the message is to startmaelstrom, start the fx
	end
	
end