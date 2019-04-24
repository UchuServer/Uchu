require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_RACE_FLAGGER_CLICKED01")) --Check in at the bottom of the monument if you want to be timed. You can race again and again.
	AddInteraction(self, "interactionAnim", "greet")
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FLAGGER_BANTER01"))  --Another fast time!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FLAGGER_BANTER02"))  --Yay!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FLAGGER_BANTER03"))  --Excellent!
end

