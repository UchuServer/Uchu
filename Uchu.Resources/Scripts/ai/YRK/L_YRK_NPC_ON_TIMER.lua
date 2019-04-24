require('State')
require('o_mis')

-----------------------------------------------------------
-- Run state
-----------------------------------------------------------
run=State.create()

run.onArrived=function(self,message)
	self:ContinueWaypoints()
	self:SetPathingSpeed{speed=5} 
end

run.onEnter=function(self) 
	--print("run enter")
	self:SetPathingSpeed{speed=5} 
	--self:PlayAnimation{animationID=9} 
end


-- commenting this out so they will run all the time now during the skunk invasion
--[[
run.onProximityUpdate=function(self, message)
	if message.status=="EXIT" and  haveStoredID(self, message.objId) then
			clearStoredID(self, message.objId)
			self:SetVar("skunkCount", self:GetVar("skunkCount")-1)
			if(self:GetVar("skunkCount") == 0) then
				setState("idle", self)
			end
		end
end
]]--

run.onExit=function(self)
	--print("run exit")
end

-----------------------------------------------------------
-- Idle state
-----------------------------------------------------------
idle = State.create()
idle.onEnter = function(self)
	--print("Idle enter")
	self:SetPathingSpeed{speed=.75} 
end

idle.onProximityUpdate=function(self, message)
	if message.status == "ENTER" and message.objId:GetFaction().faction == 1 and message.name == "WaveRadius" then
		--print("waving")
		self:PlayFXEffect{effectType = "emote"}
	else
		--onProximityUpdate(self, message)

	end
end

function onStartup(self)

	self:SetVar( "bRunning", false )
	
	UseWayPoints(self)
	self:FollowWaypoints()
	self:SetProximityRadius{radius = 20, name = "WaveRadius"}
	addState(run, "run", "run", self)
	addState(WayPointEvent, "WayPointEvent", "WayPointEvent", self)
	addState(idle, "idle", "idle", self)
	self:SetVar("WPEvent_NUM", 1)
	beginStateMachine("idle", self)
	
	 -- register ourself to be instructed later
    registerWithZoneControlObject(self)
	
	--self:SetVar("IdleTime", 120)
	--self:SetVar("PanicTime", 180)
	
	-- GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("IdleTime"), "IdleTimer", self )
end


onTimerDone = function(self, msg)
	--print ("Timer name: "..msg.name)
    --if (msg.name == "IdleTimer") then
	--     setState("run", self)
	-- 	GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("PanicTime"), "PanicTimer", self )
    --	print("NPCs should be running")
  
	--  elseif (msg.name == "PanicTimer") then
    --  setState("idle", self)
	--	GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("IdleTime"), "IdleTimer", self )
	--	print("NPCs should be walking")
	

	if msg.name == "DelayAction" then
		n = self:GetVar("WPEvent_NUM") + 1
		self:SetVar("WPEvent_NUM", n ) 
      
		-- if the skunk invasion has started, make the npc run on his path
		if ( self:GetVar( "bRunning" ) == true ) then
			setState("run", self)
			self:ContinueWaypoints()
		else
			setState("WayPointEvent", self)
		end
    
	
    elseif msg.name == "DelayActionEmote" then 
		n = self:GetVar("WPEvent_NUM") + 1
		self:SetVar("WPEvent_NUM", n ) 

		-- if the skunk invasion has started, make the npc run on his path
		if ( self:GetVar( "bRunning" ) == true ) then
			self:ContinueWaypoints()
			setState("run", self)
		else
			setState("WayPointEvent", self)
		end
    
     end

end



function onArrived(self, msg)
    if (msg.actions) then   
         Act_N = {}
         Act_V = {}
         for i = 1, table.maxn(msg.actions) do
            Act_N[i] = msg.actions[i].name    
            Act_V[i] = msg.actions[i].value    
         end
         self:SetVar("Act_N",Act_N)
         self:SetVar("Act_V",Act_V)
       
         setState("WayPointEvent", self)          
    else
         self:ContinueWaypoints();  -- Explained below
    end
   
end

function UseWayPoints(self)

    WayPointEvent = State.create()
    WayPointEvent.onEnter = function(self)
    local o = self:GetVar("WPEvent_NUM")   
         if self:GetVar("WPEvent_NUM") <= table.maxn(self:GetVar("Act_N")) then
           -- o = o + 1
            WayPointEventFunc(self, self:GetVar("Act_N")[o],self:GetVar("Act_V")[o])
         else
           self:SetVar("WPEvent_NUM", 1)   
           self:ContinueWaypoints();  -- Explained below
         end 
    end
    
	WayPointEvent.onProximityUpdate=function(self, message)
 		idle.onProximityUpdate(self, message)
	end    
end 


function WayPointEventFunc(self,name,value)
       ----------------------- delay
       if name == "delay" then
          self:StopMoving()
          GAMEOBJ:GetTimer():AddTimerWithCancel( value , "DelayAction", self )
       end 
      ----------------------- emote
      if name == "emote" then
            self:PlayAnimation{ animationID = value}
            local time = self:GetAnimationTime{  animationID = value }.time
            if time < 1 then 
                time = 1
            end
            GAMEOBJ:GetTimer():AddTimerWithCancel( time  , "DelayActionEmote", self )
      end 
      ----------------------- pathspeed
      if name == "pathspeed" then 
        self:SetPathingSpeed{speed = value}
        self:SetVar("WPEvent_NUM", self:GetVar("WPEvent_NUM") + 1 ) 
        setState("WayPointEvent", self) 
      end 

end


function onNotifyObject(self, msg)
    -- Update event state
    if (msg.name == "npc_panic") then
		--print("NPC Panic")
		self:SetVar( "bRunning", true )
		setState("run", self)
		
    elseif (msg.name == "npc_idle") then
		--print("NPC Idle")
		self:SetVar( "bRunning", false )
		setState("idle", self)
	end
	
end