require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_CONCERT_FAN_B_CLICKED01")) --Get up top and dance!
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER01"))  --Love those effects!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER02"))  --I like the fireworks.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER03"))  --Lasers are the best.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER04"))  --We need some real rock stars.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER05"))  --This place rocks!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_B_BANTER06"))  --Quit talking to me - go play an instrument!

end

