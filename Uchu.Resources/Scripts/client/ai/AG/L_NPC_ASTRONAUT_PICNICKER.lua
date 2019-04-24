require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_ASTRONAUT_PICNICKER_CLICKED01")) --Maelstrom clouds are very dangerous. If you see any, investigate with extreme caution.
	AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_ASTRONAUT_PICNICKER_BANTER01"))  --My sensors indicate a Maelstrom breach nearby.
	AddInteraction(self, "proximityText", Localize("NPC_ASTRONAUT_PICNICKER_BANTER02"))  --Data suggests the presence of a Maelstrom infestation.
	AddInteraction(self, "proximityAnim", "prox")

end
