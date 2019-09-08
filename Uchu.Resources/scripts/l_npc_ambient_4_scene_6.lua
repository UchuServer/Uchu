require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 40)
	SetProximityDistance(self, 40)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_ANYONE_HAVE_A_FRISBEE?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHAT_A_GREAT_DAY_IN_NIMBUS_PARK"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_DO_YOU_SMELL_FOOD?"))
	AddInteraction(self, "interactionText", Localize("NPC_NP_AMB_WHERE'S_THE_MUSTARD?"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_DID_YOU_SEE_THE_ANTS?"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_WHO'S_GRILLING_THE_BURGERS?"))
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_I_WANT_TO_PLAY_VOLLEYBALL"))
    
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
