require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_COWBOY_PICNICKER_CLICKED01")) --I hear tell of trouble ahead.
	AddInteraction(self, "interactionText", Localize("NPC_COWBOY_PICNICKER_CLICKED02")) -- Y'all wanna buy the Farm? Just check that vending contraption right there!
	AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_COWBOY_PICNICKER_BANTER01"))  --Oh give me a home...
	AddInteraction(self, "proximityText", Localize("NPC_COWBOY_PICNICKER_BANTER02"))  --Where the buffalo roam...
	AddInteraction(self, "proximityAnim", "prox")

end
