--------------------------------------------------------------
-- Server-side death trigger that will kill the player with a
-- specific animation. Change the Custom Variables to fit your 
-- needs.
-- mrb... 5/21/09
-- djames: updated 9/28/09
--------------------------------------------------------------
-- Custom Variables
--------------------------------------------------------------
local deathAnimation = "electro-shock-death"    -- Animation to play on the player when it dies

--------------------------------------------------------------
-- onCollisionPhantom handles the player colliding with the
-- attached object (i.e. water plane) by making them die
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
    --print("******* onCollisionPhantom (server)")
    
	local target = msg.objectID
	
	-- If a player collided with me, then do our stuff
	if target:IsCharacter().isChar then

		--print("******* RequestDie (server)")
		target:RequestDie{killerID = self, deathType = deathAnimation}
    end

	return msg
end
