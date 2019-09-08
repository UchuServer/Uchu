require('o_mis')
require('State')
function onStartup(self) 

	self:UseStateMachine{} 	
	   
	Idle = State.create()
    Idle.onEnter = function(self) 
        self:SetVar("StopAll", true)
    end   
    Idle.onArrived = function(self)

    end       
        
  
    stop = State.create()
    stop.onEnter = function(self) 
       GAMEOBJ:GetTimer():CancelAllTimers( self )
       self:SetVar("StopAll", true)

    end   
    stop.onArrived = function(self)

    end        
        
   
    spawn = State.create()
    spawn.onEnter = function(self) 
     self:SetVar("StopAll", false)
    	GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "spawnfloater", self )  
          
    end   
    spawn.onArrived = function(self)

    end 	   
      
	  addState(spawn, "spawn", "spawn", self)
	  addState(stop, "stop", "stop", self)
      addState(Idle, "Idle", "Idle", self)
      beginStateMachine("Idle", self)
      Idle.onEnter(self)
                  
          
      
end


 
 
 function onTimerDone(self,msg)
 
 	if (msg.name == "spawnfloater") then
 	
        if  self:GetVar("StopAll") == false then
            local mypos = self:GetPosition().pos
            local  PoS = getRandomPos(self,mypos,40)
            RESMGR:LoadObject { objectTemplate = 4975, x= PoS.x, y= PoS.y , z= PoS.z, owner = self} 
            local mypos = self:GetPosition().pos
            local  PoS = getRandomPos(self,mypos,40)
            RESMGR:LoadObject { objectTemplate = 4975, x= PoS.x, y= PoS.y , z= PoS.z, owner = self} 
            GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "spawnfloater", self )  
        end
 	end
 
 
 
 end
 function onNotifyObject(self, msg)
 
 	if msg.name == "spawn" then
 	
 		setState("spawn",self)
 	end
  	if msg.name == "stop" then
  	
  		setState("stop",self)
 	end
 	
 end
 
