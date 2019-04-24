--------------------------------------------------------------
-- Server script for loading the bouncer geyser when smashed.
--
-- updated jnf... 01/17/11
--------------------------------------------------------------

--------------------------------------------------------------
-- Called when object is smashed
--------------------------------------------------------------
function onDie( self, msg )

	-- get the hydrant's position
	local myPos = self:GetPosition{}.pos
	local hydrant = tostring(self:GetVar("hydrant"))
	
	local config = { { "hydrant" , hydrant }}
	
	RESMGR:LoadObject { objectTemplate = 12293,
						x = myPos.x, y = myPos.y, z = myPos.z,
						owner = self, configData = config }	
	
end
