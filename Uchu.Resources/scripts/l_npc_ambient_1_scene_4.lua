require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I'M_GETTING_READY_FOR_THE_OBSTACLE_COURSE"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I'M_TRYING_TO_BEAT_MY_BEST_TIME"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_IS_YOUR_BEST_TIME?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THE_BOUNCERS_ARE_THE_HARDEST_PART"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I_HOPE_YOU_KNOW_HOW_TO_DOUBLE_JUMP!"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_I_FEEL_ASLEEP"))
	
	AddInteraction(self, "proximityAnim", "yawn")
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_*STRETCH*"))
    
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
