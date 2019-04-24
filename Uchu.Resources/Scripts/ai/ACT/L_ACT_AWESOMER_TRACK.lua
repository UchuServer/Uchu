--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["PROJECTILE_LOT"] = 1822

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 
	
	
end


--------------------------------------------------------------
-- On Collision
--------------------------------------------------------------
function onCollision(self, msg)

	-- ignore projectiles
	if (msg.objectID:GetLOT().objtemplate == CONSTANTS["PROJECTILE_LOT"]) then
		msg.ignoreCollision = true
		return msg
	end
  
end

















