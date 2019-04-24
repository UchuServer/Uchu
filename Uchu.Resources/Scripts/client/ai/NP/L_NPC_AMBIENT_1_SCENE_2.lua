require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 15)
	
	AddInteraction(self, "mouseOverAnim", "np_greet")
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_A_GRAND_MONUMENT!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THIS_GARDEN_IN_SOOTHING"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_ARE_YOU_LOOKING_FOR_BATTERIES_TOO?"))
	
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_HI_THERE"))
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_HELLO"))
    
end


function onRenderComponentReady(self, msg) 

	-- change torso (red plain shirt)
	self:SwapDecalAndColor{decalIndex = 0, 
	                       color = 0, 
	                       bodyPiece = 1}

	-- change legs (blue pants)
	self:SwapDecalAndColor{decalIndex = 0, 
	                       color = 2, 
	                       bodyPiece = 2}
	
end