require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THIS_PLAYGROUND_IS_FUN!"))
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_CLOWNS_ARE_GREAT!"))
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_I_LIKE_ERIK_BEYER!"))
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_YEGGE_BROKE_THE_BUILD!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_HAPPENS_WHEN_I_CLICK_THE_SWITCHES?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_HAPPENS_WHEN_I_CLICK_THE_SWITCHES?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_HAPPENS_WHEN_I_CLICK_THE_SWITCHES?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_HAPPENS_WHEN_I_CLICK_THE_SWITCHES?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_DID_YOU_SEE_THOSE_SWITCHES?"))
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_I'VE_GOTTA_COLLECT_THE_GOLD_BRICKS!"))
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