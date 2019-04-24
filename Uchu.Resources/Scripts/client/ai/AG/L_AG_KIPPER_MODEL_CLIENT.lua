
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )

	registerWithZoneControlObject( self )
	
end




--------------------------------------------------------------
-- Render Ready
--------------------------------------------------------------
function onRenderComponentReady( self, msg )
	
	-- tell the zone script that the model is ready
	-- it will pass that info on to L_ZONE_AG_KIPPER_DUEL_CLIENT.lua
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID = self, args = "ModelReadyKipperDuel" }

end


