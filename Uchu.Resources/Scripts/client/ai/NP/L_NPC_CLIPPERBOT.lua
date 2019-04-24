require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	
	AddInteraction(self, "interactionText", "*ClipperBot-8000 Status: Ready*")
	AddInteraction(self, "interactionText", "Executing Trimming Procedures.")
	AddInteraction(self, "interactionText", "Batteries at Full Power.")
	AddInteraction(self, "interactionText", "Current Location: Avant Gardens.")
	
end

