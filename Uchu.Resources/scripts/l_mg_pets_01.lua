require('o_mis')
require('L_NP_NPC')

function onGetOverridePickType(self, msg)
	msg.ePickType = 2	--NPC type
	return msg
end

function onStartup(self)

	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 40)
	SetProximityDistance(self, 40)
	
	AddInteraction(self, "mouseOverEffect", "mouseover")
	
    AddInteraction(self, "proximityText", "Click ME!")

end
