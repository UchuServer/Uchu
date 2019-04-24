--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 4
--
-- On player interaction, felix will respond with help based
-- on mission status.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('client/ai/NP/L_NP_NPC')


--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

    
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetMouseOverDistance(self, 30)
	SetProximityDistance(self, 30)
	
	AddInteraction(self, "interactionText", Localize("NPC_NP_FELIX_STEP_ON_THE_BOUNCER_TO_GET_TO_THE_COURSE"))
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_FELIX_WELCOME_TO_THE_NIMBUS_PARK_OBSTACLE_COURSE"))
    
end
