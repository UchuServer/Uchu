
--------------------------------------------------------------
-- constants
--------------------------------------------------------------
CONSTANTS = {}

CONSTANTS["START_OVER_TIMER"] = 30.0	-- seconds allowed before
			-- the quickbuild is replaced with a good, pathing robotonist
			-- (without giving the player credit for fixing it )





--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self )

	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["START_OVER_TIMER"], "revertToOriginal", self )

end





--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)
    
    -- if we just hit the idle state
	if (msg.iState == 3) then
		
		-- a player just did the quickbuild.
		
		GAMEOBJ:GetTimer():CancelTimer("revertToOriginal", self)
		
		-- spawn in a fixed robotonist and get rid of the quickbuild
		ReplaceWithGoodRobotonist( self )
		
		-- TODO: give credit toward the repair 3 corrupted robotonists mission

	end
	
end 




--------------------------------------------------------------
-- spawn in a good, pathing robotonist
--------------------------------------------------------------
function ReplaceWithGoodRobotonist( self )
	
	-- TODO: spawn in a good robotonist at the first waypoint
	
	-- get rid of the quickbuild
	GAMEOBJ:DeleteObject( self )
end




--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

	if ( msg.name == "revertToOriginal")  then
	
		-- get rid of the quickbuild activator and pile of bricks
		self:DisplayRebuildActivator{ bShow = false }
	
		-- ran out of time to do the quickbuild, start over with a good robotonist
		ReplaceWithGoodRobotonist( self )
	end
	
end


