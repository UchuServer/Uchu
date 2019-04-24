require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_PIRATE_PICNICKER_CLICKED01")) --How fast can ye get to the finish line above the monument?
    AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_PIRATE_PICNICKER_BANTER01"))  --Arrrrr! I made it to the top of the monument much faster than you.
	AddInteraction(self, "proximityText", Localize("NPC_PIRATE_PICNICKER_BANTER02")) --Ye always go the long way around.
	AddInteraction(self, "proximityText", Localize("NPC_PIRATE_PICNICKER_BANTER03"))  --I'll be taking the shortcut through the tunnel next time, yarrr.
	AddInteraction(self, "proximityAnim", "prox")

end
