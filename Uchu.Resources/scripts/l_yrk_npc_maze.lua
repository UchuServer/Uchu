require('o_mis')
require('L_NP_NPC')

function onStartup(self)
	
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 15)
	
	AddInteraction(self, "mouseOverAnim", "np_greet")
	
	AddInteraction(self, "interactionText", Localize("NPC_YRK_MAZE_CERTAIN_WALLS_IN_THE_MAZE_CAN_BE_BUILT_INTO_DIFFERENT_WALLS"))
	
	AddInteraction(self, "proximityText", Localize("NPC_YRK_MAZE_THERE_IS_A_TROLL_IN_THE_MAZE_THAT_NEEDS_WATER_BADLY"))
    
end
