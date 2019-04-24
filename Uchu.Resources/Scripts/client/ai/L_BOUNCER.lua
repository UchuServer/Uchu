require('client/ai/L_BOUNCER_BASIC')

function onCollisionPhantom(self, msg)
	local target = msg.objectID
	
	bounceObj(self, target)
	
	return msg
end

