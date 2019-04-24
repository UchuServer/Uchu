require('o_mis')
require('client/ai/YRK/L_YRK_NPC_INTERACTIONS')
require('c_Zorillo')

function onStartup(self)
	                     
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 35)
	SetProximityDistance(self, 35)
		
	self:SetVar("bRenderReady", false)
	
	-- set state to No Info, waiting for state information
    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INFO"])
    
	DeactivateInteractions( self )
    
	InitializeAmbientHints( self )

	registerWithZoneControlObject(self)
    
end



--------------------------------------------------------------
-- remember this NPC's possible chat bubble messages
-- used if the NPC is clicked on during peace time
--------------------------------------------------------------
function InitializeAmbientHints( self )

	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_THE_SKUNK_BUSTERS_SEEM_TO_HAVE_A_LEAK_ON_THIER_ROOF"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_THERE_WHEN_THE_SKUNKS_INVADE,_THE_SKUNKS_BUSTERS_ARE_HERE..._KINDA"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_WE_NEED_NEW_SKUNK_BUSTERS"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_THE_BALLOON_CAN_ALSO_BE_POWERED_BY_A_FIG_WHO'S_SMELLY_WITH_SPECIAL_STINK"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_HAVE_YOU_SEEN_THE_STINK_POWERED_BALLOON_AT_THE_INVENTOR_SHOP?"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_WE_NEED_NEW_SKUNK_BUSTERS"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_RUMOR_HAS_IT_THAT_STANDING_ON_A_FOUNTAIN_SPOUT_MAKES_THE_OTHER_SPOUTS_MORE_POWERFUL"))
	AddInteraction(self, "interactionText", Localize("NPC_YRK_AMB_I_HEAR_IT_TAKES_2_FIGS_TO_GET_THE_INVENTOR_BALLOON_TO_FLY"))

end




--------------------------------------------------------------
-- Called when the render is ready on the client
--------------------------------------------------------------
function onRenderComponentReady(self, msg)
    self:SetVar("bRenderReady", true)
    
	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="ZoneStateClientObjectReady" }
    
end




--------------------------------------------------------------
-- Get the state of the zone
--------------------------------------------------------------
function GetZoneState(self)

    return self:GetVar("ZoneState")
    
end


--------------------------------------------------------------
-- Set the state of the zone
--------------------------------------------------------------
function SetZoneState(self, state)

    -- get current state
    local prevState = GetZoneState(self)
    
    self:SetVar("ZoneState", state)

    -- perform actions based on zone state
    if (prevState and prevState ~= state) then
    
		if ( state == CONSTANTS["ZONE_STATE_NO_INVASION"] ) then
			ActivateInteractions( self )

		else
			DeactivateInteractions( self )
		end
        
    end
    
end    





--------------------------------------------------------------
-- Called when object gets a notification
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- set the state
    if (msg.name == "zone_state_change") then
        SetZoneState(self, msg.param1)
    end
    
end


