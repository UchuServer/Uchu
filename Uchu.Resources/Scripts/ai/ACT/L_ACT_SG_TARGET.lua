--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

	-- pick a random explode factor
	local ran = math.random(2,2)
	
	-- set explode factor
	self:SetSmashableParams{fExplodeFactor = 2.5}
	
	--GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "ChangeSpeed",self )
	
end


--------------------------------------------------------------
-- Get the score for the target
--------------------------------------------------------------
function onGetActivityPoints(self, msg)
	local SpawnData = self:GetVar("SpawnData")
	if (SpawnData) then
		msg.points = SpawnData.sdScore
		msg.addTime = SpawnData.sdTimeScore
		--- Addscore here ***************************************************************************  
	end
	return msg
end


--------------------------------------------------------------
-- continue doign waypoints
-- @TODO: modify speed/path/etc
--------------------------------------------------------------
function onArrived(self, msg)

	-- do speed change
	ChangeSpeed(self)

	if (msg.isLastPoint == true) then
		Despawn(self)
	end
	if (msg.actions) then
	
		if msg.actions[1].name == "PlayAim" then
			local aimName =  msg.actions[1].value 
			DoObjectAction(self, "anim", aimName)
			
		end
	
	                    
                
	end
--    local i = 1
--    if(msg.actions) then   
--        while (msg.actions[i]) do
--            print(msg.actions[i].name)
--            print(msg.actions[i].value)
--            i = i + 1
--        end
--    end
    
    self:ContinueWaypoints()
    
end

--------------------------------------------------------------
-- when at last waypoint, notify the parent to respawn
--------------------------------------------------------------
function onPlatformAtLastWaypoint(self, msg)

	-- notify zone object of the mission task update (temp msg to talk to it)
	getParent(self):NotifyObject{ObjIDSender = self, name="FinishedPath"}

end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    -- Change the speed every 5 seconds
    if msg.name == "ChangeSpeed" then 
		-- pick a random speed
		local ran = math.random(1,4)
		
		self:SetPathingSpeed{ speed = ran }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "ChangeSpeed",self )
    end        
    
    if msg.name == "Despawn" then 
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:Die{ killerID = self, killType = "SILENT" }
    end  
        
end


--------------------------------------------------------------
-- Try to change the targets speed
--------------------------------------------------------------
function ChangeSpeed(self)
	
	-- get spawn data
	local SpawnData = self:GetVar("SpawnData")
	if (SpawnData) then
		
		-- should we try to change speed?
	    if ((SpawnData.sdChangeSpeed == true) and 
	        (math.random() <= SpawnData.sdSpeedChance)) then
		
			-- get a speed
			local newSpeed = (math.random() * 
				(SpawnData.sdMaxSpeed - SpawnData.sdMinSpeed)) + SpawnData.sdMinSpeed
			
			-- set the new speed
			self:SetPathingSpeed{ speed = newSpeed }
	    
	    end
	    
	end
	
end

--------------------------------------------------------------
-- Try to despawn the target
--------------------------------------------------------------
function Despawn(self)

	-- get spawn data
	local SpawnData = self:GetVar("SpawnData")
	if (SpawnData) then
		
		-- should we try to despawn?
	    if (SpawnData.sdDespawnTime == true) then
			local timeout = 1
			-- use 10 second timeout so platforms aren't killed on the client before they finish their path
			if ( SpawnData.sdMovingPlat == true ) then
				timeout = 10
			end			
			GAMEOBJ:GetTimer():AddTimerWithCancel( timeout, "Despawn",self )
	    end
	
	end

end















