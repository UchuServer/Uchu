require('o_mis')
require('L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_BEASTIE_BRICK_C_CLICKED01")) --What kind of emotes have you unlocked?
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_BEASTIE_BRICK_C_BANTER01"))  --Word.
	AddInteraction(self, "proximityText", Localize("NPC_BEASTIE_BRICK_C_BANTER02"))  --Rock on, yo.

end

