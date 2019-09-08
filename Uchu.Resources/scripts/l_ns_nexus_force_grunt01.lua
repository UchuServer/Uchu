require('o_mis')
require('L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_NS_NEXUS_GRUNT_CLICKED01")) --The Factions must unite and build a tower to regain control of the Nexus!
	AddInteraction(self, "interactionAnim", "prox")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_NS_NEXUS_GRUNT_BANTER01"))  --This is the way to the Nexus!
	AddInteraction(self, "proximityAnim", "prox")

end
