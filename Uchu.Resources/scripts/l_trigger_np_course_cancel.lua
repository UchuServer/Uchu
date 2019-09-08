--------------------------------------------------------------
-- (CLIENT SIDE) Trigger for Course Cancel
--
-- Cancels the course for the player
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

end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- if the local character is entering the trigger
	if (msg.objectID:GetID() == GAMEOBJ:GetLocalCharID()) then

        -- tell the zone object about it	
        GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_4_course_cancel" }
        	
	end

	-- ignore collisions
	msg.ignoreCollision = true
	return msg
  
end






