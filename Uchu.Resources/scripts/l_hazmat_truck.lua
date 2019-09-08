--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self) 

    registerWithZoneControlObject(self)
    
end


--------------------------------------------------------------
-- Handle notification of rebuild changes
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    GAMEOBJ:GetTimer():CancelAllTimers( self )

    -- if we just hit the idle state
	if (msg.iState == 3 and msg.iPrevState == 2) then
	
	    -- start a timer to break ourself again
	    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["HAZMAT_REBUILD_RESET_TIME"] , "BreakTimer", self )
	    
	end
	
end   


--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

	-- timer for earthquake ending
    if (msg.name == "BreakTimer") then

        self:RebuildReset()
        
    end    

end

