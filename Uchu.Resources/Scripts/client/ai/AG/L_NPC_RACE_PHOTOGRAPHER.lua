require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_RACE_PHOTOGRAPHER_CLICKED01")) --Race as often as you want. Come back any time!
	AddInteraction(self, "interactionAnim", "greet")
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_RACE_PHOTOGRAPHER_BANTER01"))  --A great shot!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_PHOTOGRAPHER_BANTER02"))  --Terrific time!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_PHOTOGRAPHER_BANTER03"))  --Nice work!
end

