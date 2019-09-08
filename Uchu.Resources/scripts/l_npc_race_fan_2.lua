require('o_mis')
require('L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_RACE_FAN_2_CLICKED01")) --Be sure to check out the concert up ahead!
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_2_BANTER01"))  --Woo hoo!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_2_BANTER02")) --Sweet!
	AddInteraction(self, "proximityText", Localize("NPC_RACE_FAN_2_BANTER03"))  --My arms are getting tired.
    
end

