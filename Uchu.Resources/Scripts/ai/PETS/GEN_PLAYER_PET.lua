require('o_mis')
require('State')


 --###########################################################
 --###              ON START UP                            ###
 --###########################################################

function onStartup(self)
   
	------------------------------------ Saved Varibales Start
    self:SetVar("wanderRadius", 15) 
    self:SetVar("WanderSpeed", 2) 

    -- Follow min and max allows the pet too rubber band whild folloing the player
    self:SetVar("followDis_min", 5 )
    self:SetVar("followDis_max", 10 )
    
    -- Distance the pet will teleport to the palyer
    self:SetVar("teleport_dis", 50 ) 
    
	-- Min Max Delay while wandering 
	self:SetVar("WanderDelayMin", 5 )
	self:SetVar("WanderDelayMax", 10 )
	
    -- Store the Players ObjID
    storeParent(self, self:GetParentObj().objIDParent)
    -- System Timer Stored
    self:SetVar("StoredTimer_1", 0 ) 
    self:SetVar("myState", nil ) 
    
     self:UseStateMachine{} 
    
	-------------------------------------  Saved Varialbes end
	
     ---------------------------------------------------------
     ---             SET STATES                            ---
     ---------------------------------------------------------
     -- PetGame.start -- 
      PetGame = State.create()
      PetGame.onEnter = function(self)
             
      end 
      PetGame.onArrived = function(self)
             
      end 
	 -- Idle.end -- 
	 ---------------------------------------------------------
     
     
	 -- Idle.start -- 
      Idle = State.create()
      Idle.onEnter = function(self)
      -- Pulse every 5 sec
         storePlayerPOS(self) -- Stor Player Pos
      -- ###########################################################################
      -- Check Distance = if to far teleport --------------------------------- ------
          if not checkDistance(self) then
            local ParentID = getParent(self)
            local ParentPos = ParentID:GetPosition().pos
            local xSplit= {x= ParentPos.x  , y= ParentPos.y , z= ParentPos.z + 5 }
            self:Teleport{ pos = xSplit}
             local rot = ParentID:GetRotation()
            self:SetRotation{ y=rot.y, x=rot.x , w=rot.w , z=rot.z  }
          end
       -----------------------------------------------------------------------------
       
       
       -- ###########################################################################
       -- Player Movement -----------------------------------------------------------
         if  checkPlayerMovement(self) and self:GetVar("myState") ~= "wander" then -- if player not moving then Wander 
            self:FollowTarget { targetID = getParent(self), radius = 3,speed = 3, keepFollowing = false }                            
            setState("wander", self) 
         elseif self:GetVar("myState") ~= "petfollow" then
            setState("petfollow", self) 
         end 
       ------------------------------------------------------------------------------

     

      -- Check Movement = if not moved wander
      

      
     local counter = self:GetInstructionCount();
     print("Counter Count....."..counter.count);   -- number of instructions in this call to lua
     print("Counter FrameCount........"..counter.frameCount); -- number of instructions this script has ran this frame
           
           GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "testTime", self )
      
             
      end 
      Idle.onArrived = function(self)
             
      end 
	 -- Idle.end -- 
     
     
     -- Players Pet Follows the Player
	 ---------------------------------------------------------
	 -- petfollow.start --   
      petfollow = State.create()
      petfollow.onEnter = function(self)
        GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self ) -- Cancle Meander timer
        self:SetVar("myState", "petfollow" ) 
        self:FollowTarget { targetID = getParent(self), radius = 8, speed = 5, keepFollowing = true }
        
      end 
      petfollow.onArrived = function(self)
        
      end 
	-- petfollow.end -- 
    ---------------------------------------------------------
    
     -- Players Pet Stays on the command of the player
	 ---------------------------------------------------------
	 -- petstay.start --   
      petstay = State.create()
      petstay.onEnter = function(self)
      
      end 
      petstay.onArrived = function(self)
             
      end 
	-- petstay.end -- 
    ---------------------------------------------------------    
    
    -- wander.start --   
      wander = State.create()
      wander.onEnter = function(self)

	  
        self:SetVar("myState", "wander" ) 
	    radius = self:GetVar("wanderRadius") 
		local  mypos = getParent(self):GetPosition().pos 
		local  PoS = getRandomPos(self,mypos,radius)
    
			self:GoTo { speed = self:GetVar("WanderSpeed"),
						target = { 
							 x = PoS.x ,
							 z = PoS.z ,
							 y = PoS.y,
					   },
			   }
                
      end 
      wander.onArrived = function(self)
	   if  not checkPlayerMovement(self) then 
	         GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
             setState("petfollow", self) 
       else 
            local ran = getRandomD(self)
            GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
            GAMEOBJ:GetTimer():AddTimerWithCancel( ran, "MeanderPause", self )
		end
      end 
	  
    -- wander.end -- 	  
    ---------------------------------------------------------
	  
	  
	  
	---------------------------------------------------------
	---             CREATE STATES                         ---
	---------------------------------------------------------	  
	  addState(Idle, "Idle", "Idle", self)
	  addState(petfollow, "petfollow", "petfollow", self) 
      addState(petstay, "petstay", "petstay", self) 
	  addState(wander, "wander", "wander", self)
	  addState(PetGame, "PetGame", "PetGame", self)
      beginStateMachine("Idle", self)
      
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
 --#### TIMER
 --###########################################################
     
onTimerDone = function(self, msg)

    if msg.name == "MeanderPause" then
        setState("wander", self)
    end
    if msg.name == "testTime" then
        setState("Idle", self ) 
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
 --#### ON CLICKED
 --###########################################################

onUse = function (self, msg)

   local targetID = msg.user 				-- Target OBJ ID 
   local tpos = targetID:GetPosition().pos  -- Target Position
   storeTarget(self, targetID) 				-- Store Target
 
end 

 --###########################################################
 --#### MIS FUNCTIONS
 --###########################################################
 
-- Return Random # for wandering 
function getRandomD(self)
             Min = self:GetVar("WanderDelayMin")
             Max = self:GetVar("WanderDelayMax")
             ran = math.random(Min,Max)
    return ran
end

-- Saves Parent ObjID
function storeParent(self, target)
    idString = target:GetID()
    finalID = "|" .. idString
    self:SetVar("My_ParentID", finalID)
end

-- Returns Parent ObjID
function getParent(self)
    targetID = self:GetVar("My_ParentID")
    return GAMEOBJ:GetObjectByID(targetID)
end
-- Func Round number
function round2(num, idp)
  return tonumber(string.format("%." .. (idp or 0) .. "f", num))
end

function checkDistance(self)
        local myTarget = getParent(self)
        local range = self:GetVar("teleport_dis")
        local myPos = Vector.new(self:GetPosition().pos)
        local hisPos = Vector.new(myTarget:GetPosition().pos)
        local dist = hisPos - myPos
        
        if dist:sqrLength() <= range * range then 
                return true
        end
        return false
end -- end CanAttackTarget

function checkPlayerMovement(self) 
    mypos = getParent(self):GetPosition().pos 
    if round2(self:GetVar("Parentx"), 1)  ~= round2(mypos.x,1) or  round2(self:GetVar("Parentx"),1) ~=  round2(mypos.z,1) then
        return true
    end
        return false         
end 

function storePlayerPOS(self)

    mypos = getParent(self):GetPosition().pos 
    self:SetVar("Parentx", mypos.x)
    self:SetVar("Parentz", mypos.z)
    
end

