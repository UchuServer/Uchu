require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "mouseOverAnim", "wave")
	
	AddInteraction(self, "interactionText", "You are in the Avant Gardens.")
	AddInteraction(self, "interactionText", "Take a look around, there are some interesting people here.")
	AddInteraction(self, "interactionText", "Have you seen the dinosaur?")
	AddInteraction(self, "interactionText", "Where did you come from?")
	
    AddInteraction(self, "proximityText", "Welcome to Nimbus Park!")
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_HI_THERE"))
	AddInteraction(self, "proximityText", Localize("NPC_GENERIC_HELLO"))
    
end

