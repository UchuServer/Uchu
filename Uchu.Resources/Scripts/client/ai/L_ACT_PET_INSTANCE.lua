require('o_mis')
require('client/ai/NP/L_NP_NPC')

--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in GF area.
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onStartup(self)

AddInteraction(self, "interactionAnim", "greet")

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end

function onClientUse(self, msg)
    local strText = "Enter Pet Ranch?"
    
	-- show a dialog box
	local args = { 	{"bShow", true},
					{"imageID", 3},
					{"callbackClient", self},
					{"text", strText},
					{"strIdentifier", "Pet_Ranch_Start"} }
					
	UI:SendMessage("DisplayMessageBox", args)
end