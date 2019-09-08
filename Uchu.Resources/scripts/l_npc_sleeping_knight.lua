require('o_mis')
require('L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech and animation
	AddInteraction(self, "interactionText", Localize("NPC_SLEEPING_KNIGHT_CLICKED01")) --zzzz. Zzz. Snort.
	AddInteraction(self, "interactionAnim", "greet")
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_SLEEPING_KNIGHT_BANTER01"))  --Zzzzz.
	AddInteraction(self, "proximityText", Localize("NPC_SLEEPING_KNIGHT_BANTER02"))  --Zzzz-zzzz.

end

