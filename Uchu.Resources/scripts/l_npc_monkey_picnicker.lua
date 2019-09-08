require('o_mis')
require('L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_MONKEY_PICNICKER_CLICKED01")) --Oooh eeeeh aahhh!
	AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_MONKEY_PICNICKER_BANTER01"))  --Ooo ooo ooo
	AddInteraction(self, "proximityText", Localize("NPC_MONKEY_PICNICKER_BANTER02")) --EEEE eee ee
	AddInteraction(self, "proximityText", Localize("NPC_MONKEY_PICNICKER_BANTER03"))  --Oooo eeee oooo
	AddInteraction(self, "proximityAnim", "prox")

end
