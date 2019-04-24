--------------------------------------------------------------
-- Responsible for registering with the client side 
-- zone object to control actions and animations.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


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
-- Generic notification message
--------------------------------------------------------------
function onNotifyObject(self, msg)
    -- Update event state
    if (msg.name == "start_event") then
		-- get position
		local mypos = self:GetPosition().pos 
	
		-- Turn on detectors
		RESMGR:LoadObject { objectTemplate = 3434, x= mypos.x, y= mypos.y - 1.2, z= mypos.z, owner = self, rw= 0.7071, rx= 0.0, ry= -0.7071, rz = 0.0 } 
		

    elseif (msg.name == "stop_event") then
       -- Turn off detectors
       local childDetector = getObjectByName(self, "Detector")
       if(childDetector) then
			childDetector:Die{killerID = childDetector, killType = "SILENT"}
		end
    end
    
end


onChildLoaded = function(self,msg)
	 if msg.templateID == 3434 then 
        storeObjectByName(self, "Detector", msg.childID)
        storeParent(self, msg.childID)
     end
end