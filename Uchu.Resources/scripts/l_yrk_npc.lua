require('State')

-----------------------------------------------------------
-- Run state
-----------------------------------------------------------
run=State.create()

run.onArrived=function(self,message)
	self:ContinueWaypoints()
	--self:SetPathingSpeed{speed=5} 
end

run.onEnter=function(self) 
	print("run enter")
	self:SetPathingSpeed{speed=5} 
	--self:PlayAnimation{animationID=9} 
end

run.onProximityUpdate=function(self, message)
	if message.status=="EXIT" and  haveStoredID(self, message.objId) then
			clearStoredID(self, message.objId)
			self:SetVar("skunkCount", self:GetVar("skunkCount")-1)
			if(self:GetVar("skunkCount") == 0) then
				setState("idle", self)
			end
		end
end

run.onExit=function(self)
	print("run exit")
end

-----------------------------------------------------------
-- Idle state
-----------------------------------------------------------
idle = State.create()
idle.onEnter = function(self)
	print("Idle enter")
	self:SetPathingSpeed{speed=.75} 
end

idle.onProximityUpdate=function(self, message)
	if message.status == "ENTER" and message.objId:GetFaction().faction == 1 and message.name == "WaveRadius" then
		print("waving")
 		self:PlayFXEffect{effectType = "emote"}
	else
		onProximityUpdate(self, message)

	end
end

function onStartup(self) 
	self:SetProximityRadius{radius=950}
--////////for NPC waving to player
	self:SetProximityRadius{radius = 10, name = "WaveRadius"} --FOVradius = 180, 
--////////
	self:SetVar("skunkCount", 0)
	UseWayPoints(self)
	self:FollowWaypoints()
	addState(run, "run", "run", self)
	addState(WayPointEvent, "WayPointEvent", "WayPointEvent", self)
	addState(idle, "idle", "idle", self)
	self:SetVar("WPEvent_NUM", 1)
	beginStateMachine("idle", self)
	
--///////////////////////////////////////
	-- Setting states using timers for now.
	self:SetVar("IdleTime", 120)
	self:SetVar("PanicTime", 180)
	GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("IdleTime"), "IdleTimer", self )

--/////////////////////////////////////
end


onTimerDone = function(self, msg)
print ("Timer name: "..msg.name)
    if (msg.name == "IdleTimer") then
        setState("run", self)
    	GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("PanicTime"), "PanicTimer", self )
    	print("NPCs should be running")
    elseif (msg.name == "PanicTimer") then
       setState("idle", self)
		GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("IdleTime"), "IdleTimer", self )
		print("NPCs should be walking")
	end


end





-----------------------------------------------------------
-- Waypoints state
-----------------------------------------------------------


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

---------------------------
-- Skunk stuff
---------------------------
function onProximityUpdate(self, message)
	if(not message.objId) then
		return
	end
	
	if IsValidSkunk(message.objId:GetLOT().objtemplate) == true then
	
		if message.status=="ENTER" then 
			self:SetVar("skunkCount", self:GetVar("skunkCount")+1)
			storeObjectID(self, message.objId)
			setState("run", self)
		else
			if haveStoredID(self, message.objId) then
				clearStoredID(self, message.objId)
				self:SetVar("skunkCount", self:GetVar("skunkCount")-1)
				if(self:GetVar("skunkCount") == 0) then
					setState("idle", self)
				end
			end
		end
	end
end

--------------------------------------------------------------
-- return if template is a valid skunk npc
--------------------------------------------------------------
function IsValidSkunk(templateID)

    -- list of npcs does not exist
    if (CONSTANTS["INVASION_SKUNK_LOT"] == nil) then
        return false
    end
	
    -- look for a valid actor
	for actors = 1, #CONSTANTS["INVASION_SKUNK_LOT"] do
		if (templateID == CONSTANTS["INVASION_SKUNK_LOT"][actors]) then
			return true
		end
	end

	return false

end


 
function onTimerDone(self, msg)
    if msg.name == "DelayAction" then
      n = self:GetVar("WPEvent_NUM") + 1
      self:SetVar("WPEvent_NUM", n ) 
     setState("WayPointEvent", self)
    end

    if msg.name == "DelayActionEmote" then 
     n = self:GetVar("WPEvent_NUM") + 1
      self:SetVar("WPEvent_NUM", n ) 
     setState("WayPointEvent", self)
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




--------------------------------------------------------------
-- store an object ID
--------------------------------------------------------------
function storeObjectID(self, object)

    idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(final, finalID)
   
end

--------------------------------------------------------------
-- have stored ID
--------------------------------------------------------------
function haveStoredID(self, object)

    targetID = self:GetVar(varName)
    if (targetID) then
		return true
	else
		return false
	end
	
end

--------------------------------------------------------------
-- get stored ID
--------------------------------------------------------------
function clearStoredID(self, object)
	idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(final, nil)
end


--------------------------------------------------------------
-- register with zone control object
--------------------------------------------------------------
function registerWithZoneControlObject(self)

    -- register with zone control object
    GAMEOBJ:GetZoneControlID():ObjectLoaded{ objectID = self, templateID = self:GetLOT().objtemplate }
	
end
