require('State')
require('o_mis')

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

	-- pick a random explode factor
	--local ran = math.random(2,2)
	
	-- set explode factor
	--self:SetSmashableParams{fExplodeFactor = 2.5}
	
	--GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "ChangeSpeed",self )
	
	  self:SetVar( "bInABubble", false ) 
      
      self:SetVar("FloatingVelocityUpdateTime", 0.1 )
      self:SetVar("StinkyVelocityUpdateTime", 0.1 )
      self:SetVar("BubblePopTime", 0.75 )
	
end


--------------------------------------------------------------
-- Get the score for the target
--------------------------------------------------------------
function onGetActivityPoints(self, msg)
	local SpawnData = self:GetVar("SpawnData")
	if (SpawnData) then
		local mod = math.floor(msg.optionalModifier / self:GetVar("streakmod") + 1)
		if mod > #self:GetVar("streakbonus") then mod = #self:GetVar("streakbonus") end
		msg.points = SpawnData.sdScore * self:GetVar("streakbonus")[mod]
		msg.addTime = SpawnData.sdTimeScore
	end
	return msg
end

function onNotifyObject(self, msg)
	if msg.name == "setpoints" then
		local SpawnData = self:GetVar("SpawnData")
		if (SpawnData) then
			SpawnData.sdScore = msg.param1
			SpawnData.sdTimeScore = msg.param2
			
			self:SetVar("SpawnData", SpawnData)
		end
	end
end
--------------------------------------------------------------
-- continue doign waypoints
-- @TODO: modify speed/path/etc
--------------------------------------------------------------
function onArrived(self, msg)

	-- do speed change
	ChangeSpeed(self)

	if (msg.isLastPoint == true) then
		--GAMEOBJ:GetZoneControlID():NotifyObject{ObjIDSender = self, name="escaped" }
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:Die{ killerID = self, killType = "SILENT"}
		--GAMEOBJ:GetZoneControlID():UpdateMissionTask {target = self, taskType = "kill"}
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
-- when killed notify the parent to respawn
--------------------------------------------------------------
function onDie(self, msg)
	-- notify zone object of the mission task update (temp msg to talk to it)
	-- note the killtype in taskType
	getParent(self):UpdateMissionTask{target = self, taskType = msg.killType}

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
  
	if (msg.name == "floatVelocityTimer") then
		UpdateFloatingVelocity(self)
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
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "Despawn",self )
	    end
	
	end

end

function WayPointEvent(self)
    local o = self:GetVar("WPEvent_NUM")   
    if self:GetVar("WPEvent_NUM") <= table.maxn(self:GetVar("Act_N")) then
        WayPointEventFunc(self, self:GetVar("Act_N")[o],self:GetVar("Act_V")[o])
    else
        self:SetVar("WPEvent_NUM", 1)
        
        if ( self:GetVar( "bInABubble" ) == false ) then  
			self:ContinueWaypoints();  -- Explained below
		end
    end 
end


function UpdateFloatingVelocity(self)

	local position = self:GetPosition{}.pos
	if ( position.y < 300 ) then				-- don't let the skunk float up out of the world bounds

		local velocity = self:GetLinearVelocity{}.linVelocity
		velocity.y = velocity.y + 15.0
		self:SetLinearVelocity{ linVelocity = velocity }
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar( "FloatingVelocityUpdateTime" ), "floatVelocityTimer", self )
	
	else
		self:Die{ killType = "SILENT" }
	end
end

function PutCleanSkunkInABubble(self, player)

	AddBubbleEffect(self)
	player:UpdateMissionTask {target = self, taskType = "complete"} --update bubble the skunk achievement
	self:StopPathing{}	-- stop following the waypoints
	self:ClearProximityRadius{}		-- get rid of the skunk's agrro radius
	
	local position = self:GetPosition{}.pos
	position.y = position.y + 5.0		-- to help the skunk break free of gravity.  Give him a headstart, and we will continually add to his velocity.
	self:SetPosition{ pos = position }
	
	GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar( "FloatingVelocityUpdateTime" ), "floatVelocityTimer", self )					
end

function AddBubbleEffect(self)

	if ( self:GetVar( "bInABubble" ) == false ) then
	
		self:ActivateBubbleBuffFromServer{ wszType = "skunk", bSpecialAnims = false }

		self:SetVar( "bInABubble", true )
		
		self:PlayAnimation{ animationID = "howl" }
	end
end

function onSquirtWithWatergun( self, msg )
	if ( self:GetVar( "bInABubble" ) == false ) then
		msg.shooterID:UpdateMissionTask {target = self, taskType = "kill"} 

		PutCleanSkunkInABubble(self, msg.shooterID)
		
		self:RemoveSkill{ skillID = 109 }

		self:StopFXEffect{ name = "stink" }

		self:ToggleActiveSkill{ iSkillID = 32, bOn = true }        --BEHAVIOR_DESTINK
	end
end
