function onDie( self, msg )

	local myPos = self:GetPosition{}.pos
	local hydrantConfig = tostring(self:GetVar("hydrant"))	
	
	local config = { { "hydrant" , hydrantConfig }}
	
	RESMGR:LoadObject { objectTemplate = 7328, 
     			    x = myPos.x, y = myPos.y, z = myPos.z,
			    owner = self, configData = config }	
end