

--------------------------------------------------------------
-- client side script bounces the player and tells the hydrant to clean off their skunk stink
--------------------------------------------------------------



--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('client/ai/L_BOUNCER_BASIC')
require('o_mis')





--------------------------------------------------------------
-- on collision functions
--------------------------------------------------------------
function onCollisionPhantom(self, msg)
	
	bounceNow( self, msg.objectID )
	
	return msg
end





--------------------------------------------------------------
-- bounces the player and tells the hydrant to clean off their skunk stink
--------------------------------------------------------------
function bounceNow( self, target )

	bounceObj(self, target)
	
	--local hydrant = getParent(self)
	local hydrant = self:GetParentObj().objIDParent
	
	if ( hydrant == nil ) then
		return
	end
	
	-- ask the hydrant to check if the player has skunk stink, and if so, wash them off
	hydrant:NotifyObject{ ObjIDSender = target, name = "cleanPlayer" }
	
end
