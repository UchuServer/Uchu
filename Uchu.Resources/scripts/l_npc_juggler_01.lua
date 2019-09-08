require('o_mis')
require('L_NP_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_CHECK_THIS_OUT!"))
	
	AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_CLICK_ME_TO_SEE_SOME_JUGGLING!"))
    
end
