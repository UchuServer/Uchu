
CONSTANTS = {}

CONSTANTS["CONTROLLER_LOT"] = 2802
CONSTANTS["PLATFORM_LOT"] = 2801
CONSTANTS["MOVINGPLATFORM_LOT"] = 2800
CONSTANTS["BOUNCER_LOT"] = 2806
CONSTANTS["SPRINGPAD_LOT"] = 2807
CONSTANTS["DOORPIECE_LOT"] = 999
CONSTANTS["SMASHABLE_LOT"] = 999

CONTROLLER = {}

PLAYER = {}


function onStartup(self, msg)
	UI:SendChat{ChatString = "zone:startup", ChatType = "LOCAL", Timestamp = 500}
	PLAYER = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )
	UI:SendChat{ChatString = "zone:player is " .. PLAYER:GetID(), ChatType = "LOCAL", Timestamp = 500}
end

-- Register important objects when loaded
function onObjectLoaded(self, msg)

	-- controller object loaded
	if (msg.templateID == CONSTANTS["CONTROLLER_LOT"]) then
		CONTROLLER = msg.objectID
	end

end

-- Relay the event to the C++ controller
function onArcadeScoreEvent(self, msg)
	local objTemplate = msg.templateID

	if( objTemplate ~= CONSTANTS["CONTROLLER_LOT"] ) then
		CONTROLLER:ArcadeScoreEvent{objectID = msg.objectID}
		--PLAYER:PlaySound{strSoundName = "UI/Vendor_Sale" }
		--PLAYER:PlayAnimation{ animationID = "mf_m_g_ball-roll-reverse" }
		--PLAYER:Teleport
	end
end
