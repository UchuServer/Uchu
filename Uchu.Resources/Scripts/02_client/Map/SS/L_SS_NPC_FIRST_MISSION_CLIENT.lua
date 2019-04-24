--------------------------------------------------------------
-- First mission. 
--
-- updated mrb... 7/5/11 - added loc string
--------------------------------------------------------------

local chatText = "SPACESHIP_INTRO_MISSION_CHAT" 
local chatTimer = 7.0
local npcCam = "LookAtFirstNPC"

function onScopeChanged(self, msg)
	if not msg.bEnteredScope or GAMEOBJ:GetControlledID():GetMissionState{missionID = 1727}.missionState > 2 then return end
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "show_bubble",self )
end

function onClientUse(self, msg)
	GAMEOBJ:GetTimer():CancelAllTimers( self )
end

function onMissionDialogueCancelled(self, msg)
	local player = GAMEOBJ:GetControlledID()

	player:PlayCinematic { pathName = npcCam,  hidePlayerDuringCine = true, lockPlayer = false}
	GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "show_bubble",self )
end 

function onTimerDone(self, msg)
	if msg.name == "show_bubble" then
		self:DisplayChatBubble{wsText = Localize(chatText)}
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( chatTimer, "show_bubble",self )
	end
end 