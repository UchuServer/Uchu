--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 5
--
-- On player interaction, felix will respond with help based
-- on mission status.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('o_MissionHelp')

-- local table to hold mission data
MISSION_DATA = {}

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

    -- register ourself to be instructed later
    registerWithZoneControlObject(self)
    
    -- load my help
    ---------------------
    
    -- Mission 1
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_1_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_THIS_GUY_NEEDS_YOUR_HELP,_TRY_CLICKING_ON_HIM") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_1_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_YOU'VE_FOUND_ALL_THE_BRICKS,_NOW_TALK_TO_HIM_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_1_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_IF_YOU_NEED_BRICKS,_TRY_SMASHING_THINGS_IN_THE_ROCK_GARDEN_UP_AHEAD") }
    end)

    -- Mission 2
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_2_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GOOD_JOB,_TALK_TO_HIM_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_2_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_NICE_JOB_BUILDING_IT,_NOW_TALK_TO_HIM_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_2_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_TRY_BUILDING_ONE_OF_THE_STATUES_IN_THE_AREA_BY_CLICKING_ON_IT") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_2_ID"], MISSION_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GREAT_JOB._FOLLOW_THE_PATH_AND_I'LL_CATCH_UP_IN_A_FEW") }
    end)

end

--------------------------------------------------------------
-- Make this object interactable
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--Interactive type
	return msg

end


--------------------------------------------------------------
-- Happens on client interaction
--------------------------------------------------------------
function onClientUse(self, msg)
    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- try mission 2
    local m2 = player:GetMissionState{ missionID = CONSTANTS["SCENE_5_MISSION_2_ID"] }
    if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_2_ID"], m2.missionState) == false) then
    
        -- try mission 1
        local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_5_MISSION_1_ID"] }
        ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_5_MISSION_1_ID"], m1.missionState)

    end
    
end

