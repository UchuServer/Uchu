--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 6
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
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_1_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_ANTS_ARE_RUINING_EVERYONE'S_PICNIC._TALK_TO_THAT_GUY_UP_AHEAD_TO_FIND_OUT_MORE") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_1_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GOOD_JOB,_NOW_TALK_TO_HIM_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_1_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_TRY_SMASHING_SOME_ANTS_IN_THE_PARK") }
    end)

    -- Mission 2
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_2_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_NOW_THAT_YOU_UNDERSTAND_COMBAT,_HEAD_UP_THE_HILL._THERE'S_SOMETHING_FISHY_GOING_ON") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_2_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_YOU'VE_STOPPED_THE_MOWERS,_NOW_RETURN_TO_THE_GUY") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_2_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_TRY_SMASHING_SOME_OF_THE_CORRUPTED_MOWERS,_BE_CAREFUL_THEY_CAN_HURT_YOU") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_2_ID"], MISSION_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GREAT_JOB._NOW_CONTINUE_ON_AND_I'LL_SEE_YOU_AROUND") }
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
    local m2 = player:GetMissionState{ missionID = CONSTANTS["SCENE_6_MISSION_2_ID"] }
    if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_2_ID"], m2.missionState) == false) then
    
        -- try mission 1
        local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_6_MISSION_1_ID"] }
        ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_6_MISSION_1_ID"], m1.missionState)

    end
    
end

