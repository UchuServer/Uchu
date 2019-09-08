require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 40)
	SetProximityDistance(self, 40)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THE_MAELSTROM_HAS_TAKEN_OVER_THE_PARK_MOWERS"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_SOMETHING_IS_WRONG_WITH_THE_PARK_MOWERS"))
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_WHAT_IS_GOING_ON?!"))
	AddInteraction(self, "interactionText", Localize("NPC_GENERIC_WHAT_ARE_WE_GOING_TO_DO?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_IS_THAT_PART_OF_THE_MAELSTROM?"))
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_WHAT_IS_GOING_ON?!"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WHY_IS_THE_GRASS_BENT?"))
    
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
