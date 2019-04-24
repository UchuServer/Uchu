
---------------------------------------------
-- When the player clicks the sign, they will be ported to their last zone 
-- and the mission will be cancelled.
----------------------------------------------
CONSTANTS = {}
CONSTANTS["SurvivalScenarioInstanceMission"] = 335

----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------
function onStartup(self) 
    
end

function onUse(self, msg)
	local user = msg.user
	user:CancelMission{missionID = CONSTANTS["SurvivalScenarioInstanceMission"] }
	user:TransferToLastNonInstance{ playerID = user, bUseLastPosition = true }
end