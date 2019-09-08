require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 40)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_YOU_CAN_CLICK_ON_EXHIBITS_TO_VOTE_FOR_THEM"))
	
	AddInteraction(self, "proximityAnim", "follow")
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WOULD_YOU_LIKE_TO_KNOW_MORE_ABOUT_EXHIBITS?"))
    
end
