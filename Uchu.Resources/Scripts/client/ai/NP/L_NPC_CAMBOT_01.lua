require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_CAMERA_READY!_NOW_POSE!"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WANT_YOUR_PICTURE_TAKEN?_CLICK_ON_ME"))
    
end

