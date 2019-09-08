require('o_mis')
require('L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 30)
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_CONCERT_FAN_A_CLICKED01")) --Did you see that guy with lots of imagination? He played forever!
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER01"))  --Love those effects!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER02"))  --I like the fireworks.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER03"))  --Lasers are the best.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER04"))  --We need some real rock stars.
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER05"))  --This place rocks!
	AddInteraction(self, "proximityText", Localize("NPC_CONCERT_FAN_A_BANTER06"))  --Why don't you try out an instrument!

end

