require('o_mis')
require('State')


 --###########################################################
 --###              ON START UP                            ###
 --###########################################################

function onStartup(self)
	------------------------------------ Saved Varibales Start
    self:SetVar("wanderRadius", 10) 
    self:SetVar("WanderSpeed", 1) 
	
	self:SetVar("WanderDelayMin", 2 )
	self:SetVar("WanderDelayMax", 5 )
	
	storeMeanderPoint(self)
	
	-------------------------------------  Saved Varialbes end
	
     ---------------------------------------------------------
     ---             SET STATES                            ---
     ---------------------------------------------------------
	 -- Idle.start -- 
      Idle = State.create()
      Idle.onEnter = function(self)
             
      end 
      Idle.onArrived = function(self)
             
      end 
	 -- Idle.end -- 
	 ---------------------------------------------------------
	 -- Pet.start --   
      pet = State.create()
      pet.onEnter = function(self)
      
      end 
      pet.onArrived = function(self)
             
      end 
	-- Pet.end -- 
    ---------------------------------------------------------
    -- wander.start --   
      wander = State.create()
      wander.onEnter = function(self)
	  
	    radius = self:GetVar("wanderRadius") 
		myPos = getMeanderPoint(self)
		PoS = getRandomPos(self,myPos,radius)
    
			self:GoTo { speed = self:GetVar("WanderSpeed"),
						target = { 
							 x = PoS.x ,
							 z = PoS.z ,
							 y = PoS.y,
					   },
			   }
                
      end 
      wander.onArrived = function(self)
	  
		local ran = getRandomD(self)
		GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
	    GAMEOBJ:GetTimer():AddTimerWithCancel( ran, "MeanderPause", self )
		
      end 
	  
    -- wander.end -- 	  
    ---------------------------------------------------------
	  
	  
	  
	---------------------------------------------------------
	---             CREATE STATES                         ---
	---------------------------------------------------------	  
	  addState(Idle, "Idle", "Idle", self)
      addState(pet, "pet", "pet", self) 
	  addState(wander, "wander", "wander", self)
      beginStateMachine("wander", self)
      
end
function getPlayer(self, num)
    targetID = self:GetVar("PlayerTarget_"..num )
    return GAMEOBJ:GetObjectByID(targetID)
end

function storePlayer(self, target, num)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("PlayerTarget_"..num , finalID)
end


 --###########################################################
 --########## PROXIMITY 
 --###########################################################

function onProximityUpdate(self, msg)

       -- ENTER
      if msg.objId:GetFaction().faction == 1  and msg.status == "ENTER" and msg.name == "petRadius" then
      
               
        end     
        -- LEAVE
       if msg.objId:GetFaction().faction == 1  and msg.status == "ENTER" and msg.name == "petRadius" then
             
             
               
       end      
      
end

 --###########################################################
 --#### TIMER
 --###########################################################
     
onTimerDone = function(self, msg)

    if msg.name == "MeanderPause" then
        setState("wander", self)
    end
    
end
 --###########################################################
 --#### EMOTE RECEIVED
 --###########################################################
 
onEmoteReceived = function(self,msg)

	local name = msg.emoteID
	local caster = msg.caster
      
end
 --###########################################################
 --#### MIS FUNCTIONS
 --###########################################################
 
function getRandomD(self)
             Min = self:GetVar("WanderDelayMin")
             Max = self:GetVar("WanderDelayMax")
             ran = math.random(Min,Max)
    return ran
end

