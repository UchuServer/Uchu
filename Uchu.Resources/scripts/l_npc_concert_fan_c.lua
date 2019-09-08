require('o_mis')
require('L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_CONCERT_FAN_C_CLICKED01")) --I love it when someone dances on the top platform. 
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER01"))  --Love those effects!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER02"))  --I like the fireworks.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER03"))  --Lasers are the best.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER04"))  --We need some real rock stars.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER05"))  --This place rocks!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_C_BANTER06"))  --Quit talking to me - go play an instrument!

end

