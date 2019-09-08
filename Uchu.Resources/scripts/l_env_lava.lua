--L_ENV_LAVA.lua

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- When objects collide with this one
--------------------------------------------------------------
function onCollision(self, msg)

--	print( "Obj Hit Me" )
	
	local target = msg.objectID
	
	-- if we want to check for player status:
--	local faction = target:GetFaction()
--	if faction and faction.faction == 1 then
--		-- object is a player
--	end

	if target:IsDead().bDead == false then
	
		-- send a message to kill the object that landed here
		target:Die{ killerID = self }
		msg.ignoreCollision = false
		
	else
	
		msg.ignoreCollision = true
		
	end
	
	return msg
	
end
