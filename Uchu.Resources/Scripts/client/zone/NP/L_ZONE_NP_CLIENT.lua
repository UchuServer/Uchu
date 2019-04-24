
-- these scripts contain specific scene functionality for the client zone object
require('client/zone/AG/L_ZONE_AG_KIPPER_DUEL_CLIENT')


-- Actor storage by scene for zone
ACTORS = {}	



--------------------------------------------------------------
-- Clears all actors data
--------------------------------------------------------------
function ClearActorsData()

	ACTORS = {}
	ACTORS["KipperDuel"] = {}

end



--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

	ClearActorsData()
		
	-- Scene Specific
    onStartupKipperDuel(self)
    
end





--------------------------------------------------------------
-- Generic message from a specific object
--------------------------------------------------------------
function onFireEvent( self, msg )
	
	-- object is telling us it is ready and to set its scene state
	-- based on the completion of a scene
		
	if ( msg.args == "ActorReadyKipperDuel" ) then
		ActorReadyKipperDuel( self, msg.senderID )
	
	elseif ( msg.args == "ModelReadyKipperDuel" ) then
		ModelReadyKipperDuel( self, msg.senderID )
		
	end
	
end

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- Scene Specific
   onChildLoadedKipperDuel( self, msg )

end






--------------------------------------------------------------
-- Timers
--------------------------------------------------------------

onTimerDone = function( self, msg )
    
	-- Scene Specific
   onTimerDoneKipperDuel( self, msg )



end
