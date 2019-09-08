require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
    AddInteraction(self, "proximityText", Localize("LUP_PORTOBELLO_INTRO_FROG_CHAT"))
end

