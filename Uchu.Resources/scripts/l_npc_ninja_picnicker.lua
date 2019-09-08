require('o_mis')
require('L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_NINJA_PICNICKER_CLICKED01")) --Successful building requires Imagination. Smash things to collect Imagination when you are low.
	AddInteraction(self, "interactionText", Localize("NPC_NINJA_PICNICKER_CLICKED02")) -- If you wish to find the Ninja Model Set, you must complete many Achievements.
	AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_NINJA_PICNICKER_BANTER01"))  --Look inside yourself and know: I am the fastest up the monument.
	AddInteraction(self, "proximityText", Localize("NPC_NINJA_PICNICKER_BANTER02")) --One who smashes will never build an understanding of the quickest path.
	AddInteraction(self, "proximityText", Localize("NPC_NINJA_PICNICKER_BANTER03"))  --You must concur your fear of heights, captain.
    AddInteraction(self, "proximityAnim", "prox")

end
