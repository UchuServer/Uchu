require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 40)
	SetProximityDistance(self, 40)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_GET_EM_OFF_ME!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I'M_SCARED_OF_BUGS!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_MY_PICNIC_IS_RUINED!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_AM_I_GOING_TO_DO?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_OH_NO!"))
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_EWW"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_ANTS_EVERYWHERE!"))
    
end

