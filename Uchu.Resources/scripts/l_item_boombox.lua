require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Boombox Script
--//   - Causes boombox to delete itself after the timer expires
--///////////////////////////////////////////////////////////////////////////////////////

-- amount of time in seconds til boombox deletes itself
local selfDestructTime = 10.0

function onStartup(self) 

	-- start a timer
	GAMEOBJ:GetTimer():AddTimerWithCancel( selfDestructTime, "boomboxTimer",self )	
	
end

onTimerDone = function(self, msg)

    -- Kill yourself
    self:Die{ killType = "SILENT" }

end 
