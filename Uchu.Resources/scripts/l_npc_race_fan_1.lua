require('o_mis')
require('L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_RACE_FAN_1_CLICKED01")) --Did you notice the golden robots hidden around the monument? 
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_1_BANTER01"))  --Even faster than the pirate!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_1_BANTER02")) --The ninja was never that fast.
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_1_BANTER03"))  --Good job!
    
end

