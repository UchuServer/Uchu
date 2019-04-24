-----------------------------------------------------------
-- this script is on the property guard in the ag property
-- he needs to go away after the player completes the last mission in this property
-- THIS SCRIPT NEEDS COMMENTING
-- brandi 6/3/10
-- updated mrb... 1/12/11 - updated for med prop
--------------------------------

local flag = { 	misID872 = 97,
				misID873 = 98,
				misID874 = 99,
				misID1293 = 118,
				misID1322 = 122
} 

function onMissionDialogueOK(self,msg)
	local player = msg.responder
	local missionID = msg.missionID
	local mission = "misID"..missionID
	-- the player accepts the mission to touch the orb, and hasnt touched the orb yet, so the mission cam
	if msg.iMissionState == 1 then
		if player:GetFlag{iFlagID = flag[mission]}.bFlag == false then
			player:PlayCinematic{ pathName = "MissionCam" }
		end
	--if the player completes the last mission, the guard should teleport away
	elseif msg.iMissionState == 4 then
		GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "GuardChat", paramObj = Guard }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "SelfDie",self )
	end
end

function onTimerDone(self,msg)
	if msg.name == "SelfDie" then
		self:RequestDie{ killerID = self, killtype = "VIOLENT" }
		
		-- get the spawner name of this object and then get the spawnerObj
		local spawnerName = self:GetVar("spawner_name") or "Guard"
		local spawnerObj = LEVEL:GetSpawnerByName(spawnerName)
		
		-- if the spawner exists then deactivate it
		if spawnerObj then
			spawnerObj:SpawnerDeactivate()
		end
	end
end