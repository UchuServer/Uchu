--L_ACT_PLAYER_DEATH_TRIGGER.lua

-- instantly kills players when they touch anything with this script on it

--------------------------------------------------------------
-- onCollision
--------------------------------------------------------------
function onCollisionPhantom(self, msg)

	local target = msg.objectID
    
	target:RequestDie{killerID = self, killType = "SILENT"}

end
