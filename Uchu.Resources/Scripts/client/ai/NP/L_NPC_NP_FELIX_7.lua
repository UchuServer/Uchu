--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 7
--
-- On player interaction, felix will respond with help based
-- on mission status.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('client/ai/NP/L_NP_NPC')

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 
	SetProximityDistance(self, 60)
	
    AddInteraction(self, "proximityText", Localize("NPC_NP_FELIX_YOU'VE_FINISHED_NIMBUS_PARK!_I_CAN_TAKE_YOU_TO_YOUREEKA"))

end

--------------------------------------------------------------
-- Make this object interactable
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--Interactive type
	return msg

end

