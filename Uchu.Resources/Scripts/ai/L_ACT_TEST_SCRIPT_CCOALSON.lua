require('o_mis')
require('client/ai/NP/L_NP_NPC')

--------------------------------------------------------------
-- Description:
--
-- Client script for PvP Lobby.
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------
CONSTANTS = {}
CONSTANTS["PVP_BASE"]     = { x = -266.0, y = 200.0, z = 270.0 }

local inLobby = false

--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onStartup(self)

	AddInteraction(self, "interactionAnim", "greet")

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end

function onClientUse(self, msg)

    local strText = "Enter PvP Lobby?"
    
	-- show a dialog box
	local args = { 	{"bShow", true},
					{"imageID", 2},
					{"callbackClient", self},
					{"text", strText},
					{"strIdentifier", "PvP_Start"} }
					
	UI:SendMessage("DisplayMessageBox", args)

end

function onMessageBoxRespond(self, msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	-- User wants to start PvP map, send him to team platform
	if (msg.iButton == 1 and msg.identifier == "PvP_Start") then
	
        self:FireEventServerSide{ senderID = msg.sender, args = 'add_Player' }
		player:SetUserCtrlCompPause{ bPaused = true }
		inLobby = true
		
	end

	-- User wants to exit PvP map
	if (msg.iButton == 1 and msg.identifier == "WaitState") then
        self:FireEventServerSide{ senderID = msg.sender, args = 'remove_Player' }
		player:Teleport{ pos = CONSTANTS["PVP_BASE"], bSetRotation = true }
		player:SetUserCtrlCompPause{ bPaused = false }
		inLobby = false
		
	end

end

function onNotifyClientZoneObject(self, msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if msg.name == "UpdateWaitStateBox" then
		--player:DisplayChatBubble{ wsText = msg.paramStr }
		local args = { 	{"bShow", inLobby},
						{"imageID", 0},
						{"callbackClient", self},
						{"text", msg.paramStr},
						{"strIdentifier", "WaitState"} }
						
		UI:SendMessage("DisplayMessageBox", args)
	end
	
	if msg.name == "CloseWaitStateBox" then
		local args = { 	{"bShow", false},
						{"imageID", 0},
						{"callbackClient", self},
						{"text", msg.paramStr},
						{"strIdentifier", "WaitState"} }
						
		UI:SendMessage("DisplayMessageBox", args)
	end
	
    if msg.name == "Update_Timer" then
			UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", inLobby }, {"itime", SecondsToClock( msg.param1 ) } })
    end

    if msg.name == "Kill_Timer" then
        UI:SendMessage( "ToggleSurvivalScoreboard", {{"visible", false }})
		player:SetUserCtrlCompPause{ bPaused = false }
    end

end

function SecondsToClock(sSeconds)

    local nSeconds = tonumber(sSeconds)
        if nSeconds == 0 then
            return "00:00"; --return "00:00:00";
        else
        nHours = string.format("%02.f", math.floor(nSeconds/3600));
        nMins = string.format("%02.f", math.floor(nSeconds/60 - (nHours*60)));
        nSecs = string.format("%02.f", math.floor(nSeconds - nHours*3600 - nMins *60));
        return nMins..":"..nSecs --return nHours..":"..nMins..":"..nSecs
    end

end
