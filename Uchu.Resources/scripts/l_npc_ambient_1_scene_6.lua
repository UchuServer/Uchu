require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 35)
	SetProximityDistance(self, 35)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THESE_ANTS_ARE_EVERYWHERE!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHERE_ARE_THEY_TAKING_MY_FOOD?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_MY_PICNIC_IS_RUINED!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_NOW_I'M_GOING_TO_BE_HUNGRY"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_MAN_I_HATE_ANTS!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHERE_IS_THE_ORKIN_MAN?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_GO_AWAY_ANTS!"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_SHOO_BUGS!"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_AHHH!_MY_FOOD!"))
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_WHAT_IS_GOING_ON?!"))
    
end

