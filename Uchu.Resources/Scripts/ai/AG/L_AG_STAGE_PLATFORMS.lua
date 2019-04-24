function onStartup(self)	
    self:StopPathing()    
end

--------------------------------------------------------------
-- upon reaching a waypoint in its path
--------------------------------------------------------------
function onArrived( self, msg )	
	if msg.wayPoint == 0 then
		self:StopPathing()
    end
end

