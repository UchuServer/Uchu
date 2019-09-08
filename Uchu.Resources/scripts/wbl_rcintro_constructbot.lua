require('o_mis')
require('L_NP_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    AddInteraction(self, "interactionText", Localize("LUP_RC_Constructbot_01"))
	
    AddInteraction(self, "proximityText", Localize("LUP_RC_Constructbot_02"))
    
end

