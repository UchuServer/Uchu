require('o_mis')
require('client/ai/AG/L_AG_NPC')

function onStartup(self)
	                       
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionAnim", "interact")
	
end

