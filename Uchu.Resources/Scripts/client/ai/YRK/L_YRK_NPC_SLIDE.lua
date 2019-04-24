require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 15)
	
	AddInteraction(self, "mouseOverAnim", "np_greet")
	
	AddInteraction(self, "interactionText", Localize("NPC_YRK_SLIDE_CLICK_ON_THE_HYDRANT_UP_THERE_TO_ACCESS_THE_SLIDE "))
	
	AddInteraction(self, "proximityText", Localize("NPC_YRK_SLIDE_HOP_ON_THIS_PLATFORM_TO_VISIT_THE_SLIDE"))
    
end