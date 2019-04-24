CONSTANTS = {}

CONSTANTS["CONTROLLER_LOT"] = 2873

CONTROLLER = {}

function onObjectLoaded(self, msg)

	-- controller object loaded
	if (msg.templateID == CONSTANTS["CONTROLLER_LOT"]) then
		CONTROLLER = msg.objectID
	
	end
 

end

-- Relay the event to the C++ controller
function onArcadeScoreEvent(self, msg)
	
	CONTROLLER:ArcadeScoreEvent{objectID = msg.objectID}
	

end
    
function onStartup(self, msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "Go", self )

	
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)

    if (msg.name == "Go") then

		UI:DisplayToolTip{strDialogText = "Follow the path. Scoring will be based on coins collected. Good luck!", strImageName = "", bShow=true, iTime=0}

	end

end