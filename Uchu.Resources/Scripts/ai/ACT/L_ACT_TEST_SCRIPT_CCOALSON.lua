--------------------------------------------------------------
-- Description:
--
-- Server script for PvP area.
-- This NPC will react to a user interaction and prompt
-- the user to start the PvP area. If the user 
-- presses yes, the NPC will send him to the PvP test instance.
--
--------------------------------------------------------------

--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
CONSTANTS = {}
CONSTANTS["TEAM1_PODIUM"] = { x = -245.0, y = 202.0, z = 191.0 }
CONSTANTS["TEAM2_PODIUM"] = { x = -197.0, y = 202.0, z = 234.0 }

local timerTick = 0
local tPlayers = {}
local playerCount = 0
local minimumPlayers = 2
local teamSender = nil

function onMessageBoxRespond(self, msg)

	if (msg.iButton == 1 and msg.identifier == "PvP_Start") then
		table.insert(tPlayers, msg.sender)
		teamSender = msg.sender
	end

end

function UpdateWaitState(self)

	local minimumCount = minimumPlayers - playerCount
	if(minimumCount == 1) then
		self:NotifyClientZoneObject{ name = "UpdateWaitStateBox" , paramStr = "Waiting for ("..minimumCount..") more player.  Press OK to exit Lobby." }
	end
	if(minimumCount > 1) then		
		self:NotifyClientZoneObject{ name = "UpdateWaitStateBox" , paramStr = "Waiting for ("..minimumCount..") more players.  Press OK to exit Lobby."}
	end
	
end

function onFireEventServerSide(self, msg)

    if msg.args == 'add_Player' then
		playerCount = playerCount + 1;
		
		local playerTeam = playerCount;
		while(playerTeam > 1) do
			playerTeam = playerTeam - 2
		end
		
		if(playerTeam == 0) then
			teamSender:Teleport{ pos = CONSTANTS["TEAM1_PODIUM"], bSetRotation = true }
		else
			teamSender:Teleport{ pos = CONSTANTS["TEAM2_PODIUM"], bSetRotation = true }
		end
		
		UpdateWaitState(self)
		
		timerTick = 10
		GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartDelay", self )
	end

    if msg.args == 'remove_Player' then
		table.remove(tPlayers, msg.senderID:GetID())		
		playerCount = playerCount - 1;
		
		if(playerCount == 0) then
			GAMEOBJ:GetTimer():CancelAllTimers(self)
		else
			timerTick = 10
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartDelay", self )
		end
		
		UpdateWaitState(self)
		
	end
	
end

function onTimerDone(self, msg)

    if msg.name == "StartDelay" then
		if(playerCount < minimumPlayers) then
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "StartDelay", self )
		else
			self:NotifyClientZoneObject{ name = "CloseWaitStateBox" }
			self:NotifyClientZoneObject{ name = "Update_Timer", param1 = timerTick }
			GAMEOBJ:GetTimer():AddTimerWithCancel(1, "ClockCountDown", self )
		end
    end
    
    if msg.name == "ClockCountDown" then
        timerTick = timerTick - 1
		if timerTick == 0 then    
			for k,v in ipairs(tPlayers) do
				v:TransferToZone{ zoneID = 420  }
			end
			self:NotifyClientZoneObject{ name = "Kill_Timer", param1 = 0 }
			GAMEOBJ:GetTimer():CancelAllTimers(self)
			playerCount = 0
		else
			self:NotifyClientZoneObject{ name = "Update_Timer", param1 = timerTick }
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "ClockCountDown", self )
		end
		
	end

end
