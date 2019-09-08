require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 15)
	
	AddInteraction(self, "mouseOverAnim", "np_greet")
	
	AddInteraction(self, "interactionText", Localize("NPC_YRK_BALLOON_GET_4_SMELLY_MINIFIGS_TO_STAND_UNDER_THE_BALLOON_INTAKES"))
	
	AddInteraction(self, "proximityText", Localize("NPC_YRK_BALLOON_I_KNOW_HOW_TO_GET_THE_BALLOON_TO_FLY"))
    
end
