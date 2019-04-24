
CONSTANTS = {}

CONSTANTS["CONTROLLER_LOT"] = 2873

CONTROLLER = {}

-- Register important objects when loaded
function onObjectLoaded(self, msg)

	-- controller object loaded
	if (msg.templateID == CONSTANTS["CONTROLLER_LOT"]) then
		CONTROLLER = msg.objectID
	--UI:SendChat{ChatString = "zone:onObjectLoaded", ChatType = "LOCAL", Timestamp = 500}
	end
 

end

-- Relay the event to the C++ controller
function onArcadeScoreEvent(self, msg)
	
	CONTROLLER:ArcadeScoreEvent{objectID = msg.objectID}
	--UI:SendChat{ChatString = "zone:onArcadeScoreEvent", ChatType = "LOCAL", Timestamp = 500}

end
