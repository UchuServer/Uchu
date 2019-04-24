--------------------------------------------------------------
-- (CLIENT SIDE) Generic script for actors in scene 2
--
-- Responsible for registering with the client side 
-- zone object to control actions and animations.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

    -- register ourself to be instructed later
    registerWithZoneControlObject(self)

end
