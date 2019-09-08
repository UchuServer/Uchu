--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 2
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
    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_AVAILABLE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_THIS_GUY_NEEDS_YOUR_HELP,_TRY_CLICKING_ON_HIM") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_READY_TO_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_NOW_THAT_YOU_HAVE_FOUND_ALL_THE_BATTERIES,_CLICK_ON_HIM_AGAIN") }
    end)

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_COMPLETE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_FOLLOW_THE_PATH_ACROSS_THE_BRIDGE,_I'LL_CATCH_UP") }
    end)    

    AddMissionHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], MISSION_ACTIVE, function(self)
        self:DisplayChatBubble{ wsText = Localize("NPC_NP_FELIX_THE_BATTERIES_ARE_AROUND_HERE_SOMEWHERE") }
    end)

end


--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

	self:SetProximityRadius { radius = CONSTANTS["FELIX_2_PROX_RADIUS"] }

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
    
    -- try mission 1
    local m1 = player:GetMissionState{ missionID = CONSTANTS["SCENE_2_MISSION_1_ID"] }
    ActivateHelp(self, MISSION_DATA, CONSTANTS["SCENE_2_MISSION_1_ID"], m1.missionState)
    
end


--------------------------------------------------------------
-- Called when an entity gets within proximity of the object
--------------------------------------------------------------
function onProximityUpdate(self, msg)

    -- if the local player left our radius, try showing him the chat help
	if (msg.status == "LEAVE") and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then
	
		ShowChatHelp(self)

	end

end


--------------------------------------------------------------
-- Check player and show chat help if player has not seen
--------------------------------------------------------------
function ShowChatHelp(self)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	if ( (player) and (player:Exists()) ) then

		-- check the tooltip flag to see if he has done this event
		local tooltipMsg = player:GetTooltipFlag{ iToolTip = CONSTANTS["PLAYER_CHAT_HELP_FLAG_BIT"] }
		if ((tooltipMsg) and (tooltipMsg.bFlag == false)) then

            -- show tooltip
            UI:DisplayToolTip
            {
                strDialogText = CONSTANTS["SCENE_2_CHAT_HELP_TEXT"], 
                strImageName = "", 
                bShow=true, 
                iTime=5000
            }            
            
            -- flag the tooltip bit
            player:SetTooltipFlag{ iToolTip = CONSTANTS["PLAYER_CHAT_HELP_FLAG_BIT"], bFlag = true }
            
		end    
		
	end

end
