require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 20)
	
	AddInteraction(self, "interactionText", "I hope no one presses that red button over there.")
	AddInteraction(self, "interactionText", "Is that all the troll can carry?")
	AddInteraction(self, "interactionText", "Did you check the collision on these fences? I heard Sean W. exported them.")
	AddInteraction(self, "interactionText", "This area is under construction.")
	AddInteraction(self, "interactionText", "Watch your step. This area is under construction.")
	
    AddInteraction(self, "proximityText", "No, no, no ... over there!")
	AddInteraction(self, "proximityText", "Break time is over!")
	AddInteraction(self, "proximityText", "We gotta get this building done.")
    
end

