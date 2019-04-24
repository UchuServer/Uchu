--------------------------------------------------------------
-- (CLIENT SIDE) Script for the mission giver in 
-- scene 2
--
-- Responsible for registering with the client side 
-- zone object to control actions and animations.
-- and passing data to the zone object when the local client
-- completes/accepts missions.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('client/ai/NP/L_NP_NPC')
require('o_MissionHelp')

-- local table to hold mission data
MISSION_DATA = {}

--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

    -- register ourself to be instructed later
    registerWithZoneControlObject(self)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_GENERIC_THANKS_FOR_YOUR_HELP") }
    end)    

	SetProximityDistance(self, 30)
	
    AddInteraction(self, "proximityAnim", "attention")
    
end


--------------------------------------------------------------
-- Called when client clicks OK on a mission dialog
--------------------------------------------------------------
function onMissionDialogueOK(self, msg)

    -- on mission 1 complete (collectible batteries)
    if (msg.missionID == CONSTANTS["SCENE_2_MISSION_1_ID"] and msg.bIsComplete == true) then

        GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_2_mission_1_complete" }
    
    end

end


--------------------------------------------------------------
-- Called when client clicks on the npc
--------------------------------------------------------------
function onClientUse(self, msg)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- try mission 1
    local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_2_MISSION_1_ID"] }
    ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], m1.missionState)

end