--------------------------------------------------------------
-- (SERVER SIDE) Script for animated dwarf/troll in scene 1
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)

	self:FollowWaypoints()
	
end

--------------------------------------------------------------
-- When object arrives at a waypoint
--------------------------------------------------------------
function onArrived(self, msg)

	if msg.actions and msg.actions[1].name == "animate" then

        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "DoClean",self )
        
    else
        self:ContinueWaypoints()
	end

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- keep moving
    if (msg.name == "StartMoving") then
	    
	    self:ContinueWaypoints()

    elseif (msg.name == "DoClean") then
    
        local animTime = self:GetAnimationTime{animationID = "clean"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( animTime.time + 1.0, "StartMoving",self )		
        self:PlayAnimation{animationID = "clean"}
        
    end
	
end