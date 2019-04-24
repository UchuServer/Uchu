require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	-- When mousing over
	AddInteraction(self, "mouseOverAnim", "wave")
	
    -- Click on speech
	AddInteraction(self, "interactionText", Localize("NPC_FRIENDLY_FELIX_CLICKED01")) --Keep collecting bricks. They'll come in handy once you get to Nimbus Station!
	AddInteraction(self, "interactionAnim", "greet")
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("NPC_FRIENDLY_FELIX_BANTER01"))  --Ninjas, pirates, astronauts, cowboys!
	AddInteraction(self, "proximityText", Localize("NPC_FRIENDLY_FELIX_BANTER02")) --You can be anything in LEGO Universe.
	AddInteraction(self, "proximityText", Localize("NPC_FRIENDLY_FELIX_BANTER03"))  --I march to my own beat... but there's a killer concert up ahead.
	AddInteraction(self, "proximityAnim", "prox")

end

