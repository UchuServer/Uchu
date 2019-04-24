require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)

    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_ROBOT_PICNICKER_CLICKED01")) --The concert requires large quantities of imagination. Smash objects to recharge Imagination.
	AddInteraction(self, "interactionText", Localize("NPC_ROBOT_PICNICKER_CLICKED02")) --You may buy the Mech Bay Model Set from the vending machine behind me.
	AddInteraction(self, "interactionAnim", "greet")

	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_ROBOT_PICNICKER_BANTER01"))  --Insect life forms. Why does it always have to be insect life forms?
	AddInteraction(self, "proximityText", Localize("NPC_ROBOT_PICNICKER_BANTER02")) --00011100 0011000111  01100
	AddInteraction(self, "proximityText", Localize("NPC_ROBOT_PICNICKER_BANTER03"))  --I hate getting bugs in my system.
	AddInteraction(self, "proximityAnim", "prox")

end

