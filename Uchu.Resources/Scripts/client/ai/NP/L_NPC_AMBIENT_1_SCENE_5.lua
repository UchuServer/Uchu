require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_TRY_SMASHING_SOME_OF_THE_ROCKS_IN_THE_ROCK_GARDEN"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THESE_STATUES_ARE_GREAT"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_THIS_FOUNTAIN_IS_SOOTHING"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_ARE_YOU_HAVING_A_GOOD_TIME_IN_NIMBUS_PARK?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_GET_THE_BRICK?"))
	
	AddInteraction(self, "proximityAnim", "cheer")
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WELCOME_TO_NIMBUS_PARK!"))
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_WONDERLAND_IS_GREAT!"))
    AddInteraction(self, "proximityText", Localize("NPC_GENERIC_GOOD_AFTERNOON"))
    
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
