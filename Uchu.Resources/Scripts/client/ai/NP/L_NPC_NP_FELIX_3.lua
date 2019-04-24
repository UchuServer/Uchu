--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 3
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
    
    -- Mission 1 (talk to sherland)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_1_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_THE_BEASTIE_BLOCKS_NEED_YOUR_HELP,_TALK_TO_THE_KID_ON_STAGE") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_1_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_LOOK_AROUND_FOR_OLD_MAN_SHERLAND") }
    end)

    -- Mission 2 (get earphones)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_2_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_TALK_TO_OLD_MAN_SHERLAND_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_2_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GIVE_THE_EARPHONES_TO_OLD_MAN_SHERLAND") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_2_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_SMASH_SOME_CRATES_TO_FIND_OLD_MAN_SHERLAND'S_EARPHONES") }
    end)    
    
    
    -- Mission 3 (return cd)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_3_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_TALK_TO_OLD_MAN_SHERLAND_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_3_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GIVE_THE_CD_TO_THE_KID_ON_STAGE") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_3_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GIVE_THE_CD_TO_THE_KID_ON_STAGE") }
    end)        
    
    
    -- Mission 4 (use emote)
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_THE_KID_ON_STAGE_WANTS_TO_TALK_TO_YOU_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_NICE_MOVES,_TALK_TO_THE_KID_ON_STAGE_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_STAND_ON_STAGE_AND_TYPE_'/DANCE'_IN_THE_CHATBOX,_THEN_PRESS_ENTER") }
    end)        
    
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_GOOD_JOB,_CONTINUE_DOWN_THE_PATH_AND_I'LL_CATCH_UP") }
    end)    
    

end


--------------------------------------------------------------
-- Called when this object is ready to render
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="SceneActorReady" }

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
    
    -- start with mission 4
    local m4 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_4_ID"] }
    if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], m4.missionState) == false) then

        -- try mission 3
        local m3 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_3_ID"] }
        if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_3_ID"], m3.missionState) == false) then

            -- try mission 2
            local m2 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_2_ID"] }
            if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_2_ID"], m2.missionState) == false) then
        
                -- try mission 1
                local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_1_ID"] }
                ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_1_ID"], m1.missionState)
            
            end

        end
   
    end
 
end


