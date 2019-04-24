--------------------------------------------------------------
-- (CLIENT SIDE) Scenario Exit
--
-- Handles client side dialogs/messages and input
--------------------------------------------------------------

----------------------------------------------------------------
-- Includes
----------------------------------------------------------------
--require('o_mis')
require('c_AvantGardens')



----------------------------------------------------------------
-- Startup of the object
----------------------------------------------------------------
function onStartup(self) 
    
end


--------------------------------------------------------------
-- Make this object interactable (must register for picking)
--------------------------------------------------------------
function onGetOverridePickType(self, msg)
	msg.ePickType = 14	--Interactive type
	return msg

end


----------------------------------------------------------------
-- Happens on client interaction (must register for picking)
----------------------------------------------------------------
function onClientUse(self, msg)
end


