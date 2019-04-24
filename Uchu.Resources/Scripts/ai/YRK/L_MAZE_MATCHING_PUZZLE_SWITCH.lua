
CONSTANTS = {}
CONSTANTS["radius"] = 5


function onStartup(self)
	
	self:SetProximityRadius { radius = CONSTANTS["radius"] }

end

