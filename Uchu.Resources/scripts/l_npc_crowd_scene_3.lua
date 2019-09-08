--------------------------------------------------------------
-- (CLIENT SIDE) Generic script for actors in scene 3
--
-- Responsible for registering with the client side 
-- zone object to control actions and animations.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('L_NP_NPC')
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

   -- load my help
    ---------------------
     AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_READY_TO_COMPLETE, function(self)
        local doNothing = 0
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], MISSION_COMPLETE, function(self)
        local doNothing = 0
    end)    
    
	SetProximityDistance(self, 30)
	    
end


--------------------------------------------------------------
-- Make this object interactable
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--Interactive type
	return msg

end

--------------------------------------------------------------
-- Display a chat bubble
--------------------------------------------------------------
function ShowChatBubble(self)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- try mission state
    local m4 = player:GetMissionState{ missionID = CONSTANTS["SCENE_3_MISSION_4_ID"] }
    if (ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_3_MISSION_4_ID"], m4.missionState) == false) then

        -- mission not complete
        local num = math.random(1,#CONSTANTS["CROWD_NO_MISSION_TEXT"])
        self:DisplayChatBubble{wsText = CONSTANTS["CROWD_NO_MISSION_TEXT"][num]}
   
    else
    
        -- mission complete
        local num = math.random(1,#CONSTANTS["CROWD_YES_MISSION_TEXT"])
        self:DisplayChatBubble{wsText = CONSTANTS["CROWD_YES_MISSION_TEXT"][num]}

    end
	
end


--------------------------------------------------------------
-- Happens on client interaction
--------------------------------------------------------------
function onClientUse(self, msg)
    
    ShowChatBubble(self)

end


--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    if ( (player) and (player:Exists()) ) then
    
        local  ran =  math.random(1,100)
        if ran <= CONSTANTS["CROWD_PROX_CHANCE"] and msg.status == "ENTER" and msg.objId:GetID() == player:GetID() then    
			ShowChatBubble(self)
			self:PlayFXEffect{ name = "crowd", effectID = CONSTANTS["SCENE_3_CROWD_EFFECT_ID"], effectType = CONSTANTS["SCENE_3_CROWD_EFFECT_TYPE"] }
		end
	end
end
