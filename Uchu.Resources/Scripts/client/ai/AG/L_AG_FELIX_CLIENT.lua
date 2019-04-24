
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('o_MissionHelp')



-- copied from L_NPC_TOWN_SQUARE in the yrk client scripts
require('client/ai/YRK/L_YRK_NPC_INTERACTIONS')
require('c_Zorillo')

function onStartup(self)
	                     
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 20)
	SetProximityDistance(self, 35)
		
	self:SetVar("bRenderReady", false)
    
	InitializeAmbientHints( self )

	ActivateInteractions( self )

    
end



--------------------------------------------------------------
-- remember this NPC's possible chat bubble messages
-- used if the NPC is clicked on during peace time
--------------------------------------------------------------
function InitializeAmbientHints( self )

	AddInteraction(self, "interactionText", Localize("NPC_AG_SHORTCUT_HINT"))
	AddInteraction(self, "interactionText", Localize("NPC_AG_JUMPING_HINT"))
	AddInteraction(self, "interactionText", Localize("NPC_AG_COLLECTIBLE_HINT"))


end
