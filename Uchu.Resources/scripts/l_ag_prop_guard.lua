-----------------------------------------------------------
-- this script is on the property guard in the ag property
-- he needs to go away after the player completes the last mission in this property
-- THIS SCRIPT NEEDS COMMENTING
-- brandi 6/3/10
-- updated mrb... 7/27/11 - updated for 1.9 existingplayerupdate
--------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_REMOVE_RENTAL_GEAR')

local firstMission	= 891 -- make sure the player has started the guards first mission, workaround for 1.9
local lastMission = 320

function onMissionDialogueOK(self,msg)
	local player = msg.responder
	local mState = player:GetMissionState{missionID = lastMission}.missionState
	
	-- the player accepts the mission to touch the orb, and hasnt touched the orb yet, so the mission cam
	if msg.missionID == 768 and msg.iMissionState == 1 then
		if player:GetFlag{iFlagID = 71}.bFlag == false then
			player:PlayCinematic{ pathName = "MissionCam" }
		end
		
	-- if the player accepts the last tutorial mission, let the zone control object know
	-- and the guard should teleport away
	elseif (msg.missionID == lastMission and msg.iMissionState == 1) or (mState == 8 and msg.missionID == firstMission and msg.iMissionState == 4) then		
	    GAMEOBJ:GetZoneControlID():NotifyObject{ name = "LastMissionAccepted", param1 = player:GetID(), ObjIDSender = player  }
	    
		-- player flag for being able to open the property up to visitors
		player:SetFlag{iFlagID = 113, bFlag = true}  
		GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "GuardChat", paramObj = Guard }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "SelfDie",self )
		
	end
	
	baseMissionDialogueOK(self, msg)
end

function onTimerDone(self,msg)
	if msg.name == "SelfDie" then
		self:RequestDie{ killerID = self, killtype = "VIOLENT" }
		LEVEL:GetSpawnerByName("PropertyGuard"):SpawnerDeactivate()
	end
end
