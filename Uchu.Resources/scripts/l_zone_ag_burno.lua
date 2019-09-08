
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')






--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartupBurno( self )

	self:SetVar( "bLoadedHotdogCart", false )
end





--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoadedBurno( self, msg )

      
	if ( msg.templateID == CONSTANTS["LOT_PATH_UNDER_BURNO"] ) then
		
		storeObjectByName( self, "PathUnderBurno", msg.objectID )
		checkIfReadyForHotdogCart( self )

		
		
	elseif ( msg.templateID == CONSTANTS["LOT_BURNO"] ) then
		
		storeObjectByName( self, "Burno", msg.objectID )
		checkIfReadyForHotdogCart( self )
	
	end
		
end





--------------------------------------------------------------
-- checks if both Burno and the path under him are loaded
-- if so, we can tell Burno to go ahead and spawn the hotdog cart
--------------------------------------------------------------
function checkIfReadyForHotdogCart( self )

	if ( self:GetVar( "bLoadedHotdogCart" ) == true ) then
		return
	end
	
	
	local burno = getObjectByName( self, "Burno" )
	local path = getObjectByName( self, "PathUnderBurno" )
	
	
	if (burno ~= nil and path ~= nil ) then
		
		self:SetVar( "bLoadedHotdogCart", true )
		burno:NotifyObject{ name="loadCart" }
		
	end
		
		
end

