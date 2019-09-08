--------------------------------------------------------------
-- (CLIENT SIDE) Generic script for actors in scene 3
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


--------------------------------------------------------------
-- Called when this object is ready to render
--------------------------------------------------------------
function onRenderComponentReady(self, msg)

	-- let the zone control object know we are ready to be instructed
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID=self, args="SceneActorReady" }

end



