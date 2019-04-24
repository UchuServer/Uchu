-- Script that can be attached to a trigger volume to play the opening cinematic
-- on the space ship.
-- Updated 7/13/11 mrb... using handleInteractionCamera instead of playCinematic
--//////////////////////////////////////////////////////////////////////////////////
local cinemaName = 'Flythrough' -- name of cinematic in HF
local npcCam = "LookAtFirstNPC"
local finishCam = "LookAtBob"
local tutDelay = '2' -- time after player is released from cinematic before displaying the wasd tut
--////////////////////////////////////////////////////////////////////////////////// 1727

function onCollisionPhantom(self, msg)
	-- Don't do this unless it's the local player
	if GAMEOBJ:GetLocalCharID() ~= msg.objectID:GetID() then return end
	
	local player = msg.objectID
	
	if player:GetMissionState{missionID = 1727}.missionState > 2 then return end 
		
	-- freeze player and play cinematic and sounds		
	player:SetStunned{ StateChangeType = "PUSH", bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	-- play the cinematic, but dont lock the player
	player:PlayCinematic { pathName = cinemaName, lockPlayer = false}	
	-- request notification from the player when the cinematic is done
	self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName = "CinematicUpdate"}
	
	-- change ui state to CinematicBars
	UI:SendMessage( "pushGameState", {{"state", "cinematic" }} )
end

function notifyCinematicUpdate(self, zoneObj, msg)
	-- if the cinematic isn't ending then do nothings
	if msg.event ~= "ENDED" then return end
	
	if msg.pathName == cinemaName then
		-- cancel notification
		self:SendLuaNotificationCancel{requestTarget = zoneObj, messageName = "CinematicUpdate"}
		
        GAMEOBJ:GetControlledID():SetStunned{StateChangeType = "POP", bCantInteract = true}
		-- start look at timer
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "lookAt", self)
	elseif msg.pathName == finishCam then -- recieved from the onAccept camera from the first mission
		-- remove the stunned state
		GAMEOBJ:GetControlledID():SetStunned{ StateChangeType = "POP", bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }
        -- display tooltip timer
        GAMEOBJ:GetTimer():AddTimerWithCancel( tutDelay, "displayWASD",self )            
		-- cancel notification
		self:SendLuaNotificationCancel{requestTarget = zoneObj, messageName = "CinematicUpdate"}
	end
end

function onTimerDone (self,msg)
	local player = GAMEOBJ:GetControlledID()

    if msg.name == "lookAt" then           
		-- change ui state to normal
		UI:SendMessage( "popGameState", {{"state", "cinematic"}} )
		-- play the cinematic, this message also hides any players and npcs nearby the target ID
		player:HandleInteractionCamera{wsCameraPath = npcCam, actionType = "ACTION_TYPE_TRANSISTION" }
		self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID(), messageName = "CinematicUpdate"}
    elseif msg.name == "displayWASD" then
        player:Help{ iHelpID = 6 }
    end
end 