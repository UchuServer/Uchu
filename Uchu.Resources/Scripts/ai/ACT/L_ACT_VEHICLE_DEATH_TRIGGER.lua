--L_ACT_PLAYER_DEATH_TRIGGER.lua

-- instantly kills players when they touch anything with this script on it

--------------------------------------------------------------
-- onCollision
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
	print ("Car collided with DeathPlane")
	local target = msg.objectID


	-- If a player collided with me, then do our stuff
	if target:BelongsToFaction{factionID = 113}.bIsInFaction and target:IsDead().bDead == false then
		target:Die{killerID = self}
    end

	return msg
end
