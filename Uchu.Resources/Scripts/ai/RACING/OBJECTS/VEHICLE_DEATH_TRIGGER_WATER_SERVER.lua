--------------------------------------------------------------
-- Server-side death trigger that instantly kills vehicles 
-- when they touch anything with this script on it
--------------------------------------------------------------

--------------------------------------------------------------
-- Custom Variables
--------------------------------------------------------------
local deathFXname = "death_water"	-- FX and/or Animation to play when the vehicle dies

--------------------------------------------------------------
-- onCollision
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
	print ("Car collided with DeathPlane")
	local target = msg.objectID

	-- If a player collided with me, then do our stuff
	if target:BelongsToFaction{factionID = 113}.bIsInFaction and target:IsDead().bDead == false then
		target:Die{killerID = self, deathType = deathFXname}
    end

	return msg
end
