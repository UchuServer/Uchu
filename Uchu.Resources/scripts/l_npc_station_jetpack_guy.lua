require('o_mis')
require('L_NP_NPC')

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 25)

	--AddInteraction(self, "proximityText", "Can you help me turn this jetpack on?")
	--AddInteraction(self, "proximityText", "Can you turn this jetpack on for me?")
	AddInteraction(self, "interactionAnim", "interact")
    
end

