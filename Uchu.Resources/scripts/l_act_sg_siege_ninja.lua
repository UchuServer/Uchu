require('o_mis')

local delay_time = 1.5

function onStartup(self) 

	-- pick a random explode factor
	local ran = math.random(1,5)
	
	-- set explode factor
	self:SetSmashableParams{fExplodeFactor = ran}

	-- set the current waypoint number
	self:SetVar("CurWaypoint", 1)
	
    -- delay to move somewhere
    GAMEOBJ:GetTimer():AddTimerWithCancel( delay_time, "MoveToWP",self )

end


function onArrived(self, msg)

	local curWP = self:GetVar("CurWaypoint")
	
	if (curWP <= 3) then
	   -- for some reason there needs to be a delay when an NPC arrives at a waypoint
	   -- otherwise some actions and animations will get overriden or will not play
	   GAMEOBJ:GetTimer():AddTimerWithCancel( delay_time, "MoveToWP",self )  
	else
		-- forward this onto the parent
		getParent(self):Arrived()
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "TimerDeleteObject",self )
		--self:KillObj{targetID = self}
		
	end
	
end

function onDie(self, msg)

	-- notify zone object of the mission task update (temp msg to talk to it)
	getParent(self):UpdateMissionTask{target = self}

end

function onGetActivityPoints(self, msg)
	local SpawnData = self:GetVar("SpawnData")
	if (SpawnData) then
		msg.points = SpawnData.sdScore
	end
	return msg
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    if msg.name == "MoveToWP" then 
    
		-- get waypoint data
		local curWP = self:GetVar("CurWaypoint")
		
		-- get spawn data
		local SpawnData = self:GetVar("SpawnData")	
		
		local dest = self:GetVar("Waypoint" .. curWP)
		
		if (SpawnData and dest) then
			
			-- set the current waypoint number
			curWP = curWP + 1
			self:SetVar("CurWaypoint", curWP)
			
			-- move to the waypoint
			self:GoTo{speed = SpawnData.sdSpeed, target = dest}
			
		end
		
    end
    
    if msg.name == "TimerDeleteObject" then 
		-- @TODO: Is this the best way to delete an object from script??
		--        This generates a bunch of messages about OffCollision etc...
		getParent(self):UpdateMissionTask{target = self}
		self:MoveToDeleteQueue{}
    end        
    
end


















