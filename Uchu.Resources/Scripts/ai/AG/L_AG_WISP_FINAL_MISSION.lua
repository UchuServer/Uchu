
---------------------------------------------
-- When the player accepts the final WISP mission in ag,
-- the player will zone to an instance of the mission.
----------------------------------------------
CONSTANTS = {}
CONSTANTS["SurvivalScenarioInstanceMission"] = 335

function onMissionDialogueOK(self, msg)
	local user = msg.responder
	local mission = msg.missionID 
	local missionstate = user:GetMissionState{missionID = CONSTANTS["SurvivalScenarioInstanceMission"] }.missionState
	if mission == CONSTANTS["SurvivalScenarioInstanceMission"] and (missionstate == 2 or missionstate == 10) then
	    user:TransferToZone{ zoneID = 430, ucInstanceType = 1 }
	end
end