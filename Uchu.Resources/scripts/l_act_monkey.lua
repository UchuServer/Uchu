--------------------------------------------------------------
-- (SERVER SIDE) Script for monkey in minigame
--
-- Reports proximity events to the parent
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	if msg.status == "ENTER" then 

        -- forward the event to the parent
        getParent(self):FireEvent{args = "monkey_prox", senderID = msg.objId}
		
	end

end


--------------------------------------------------------------
-- continue doign waypoints
-- @TODO: modify speed/path/etc
--------------------------------------------------------------
function onArrived(self, msg)
    
    -- forward the event to the parent
    getParent(self):FireEvent{args = "monkey_arrived", senderID = self}
    
end


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)
	
end


function onTimerDone(self, msg)

    if (msg.name == "PlayWinAnimation") then
        self:PlayAnimation{animationID = "interact"}
    elseif (msg.name == "PlayLoseAnimation") then
        self:PlayAnimation{animationID = "death"}
    end    

end
