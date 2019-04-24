require('State')
require('o_WayPointEvents')
require('o_onEvent')
require('o_mis')
require("o_Movement")


function CreateStates(self)
      
      Timers = {}
      Timers[1] = "blank"  
      self:SetVar("Timers",Timers)

      storeHomePoint(self)
      self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = 0 }
      
      CombatState(self)
      
     -- //////////////////////////////////////////////////////////////////////////////////
     -- if Way Points
     if self:GetVar("Set.WayPointSet") ~= nil or self:GetVar("attached_path") ~= nil or self:GetVar("groupID") ~= nil then
        UseWayPoints(self)
     end 
     -- if PetCalss
    if self:GetVar("Set.Pet_Active") then   
        SetPetClass(self)
    end
    -- if Frozen
     if self:GetVar("Set.MovementType") == "Frozen" then
         AggroState(self)
     end

	
     -- //////////////////////////////////////////////////////////////////////////////////
     -- if Guard
     if self:GetVar("Set.MovementType") == "Guard" then
         AggroState(self)
     end
     
     -- //////////////////////////////////////////////////////////////////////////////////
     -- if Wander
     if self:GetVar("Set.MovementType") == "Wander" then
        MeanderState(self)
     end
     
     -- //////////////////////////////////////////////////////////////////////////////////
     -- if follow
     if self:GetVar("Set.FollowActive")  then
        FollowState(self)
     end     
    
     
     -- //////////////////////////////////////////////////////////////////////////////////
     -- if Helper 
     if self:GetVar("Set.Helper") then
        SetHelper(self)  
     end
           customState = State.create()
	     
	     customState.onEnter = function(self)
	         if (onTemplateCustomStateEnter) and (onTemplateCustomStateEnter(self, msg) == true ) then
	     	    return 
	         end 
	     end
	     customState.onArrived = function(self, msg)
	         if (onTemplateCustomStateArrived) and (onTemplateCustomStateArrived(self, msg) == true  ) then
	     	    return 
	         end          
    end 
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Idle
    Idle = State.create()
  
    Idle.onEnter = function(self) 
       
        if self:GetVar("Set.Pet_Active") and not self:GetVar("PetLoaded") then
            self:SetVar("PetLoaded", true)  
            setState("PetClass",self)    
        end 
       
        if  self:GetVar("RebuildStart") then
           
            storeHomePoint(self)
            self:SetVar("inpursuit",true)
            GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.Emote_EventDelay") , "rebuildBreak", self )
        end

        
    end

    AiDisable = State.create()
    AiDisable.onEnter = function(self)
        self:SetVar("myTarget", nil)
        storeHomePoint(self)
        self:SetTetherPoint { tetherPt = self:GetPosition().pos,radius = 0 }
        CancelTemplateTimers(self)
        self:SetVar("AiDisabled", true)
        self:StopPathing()
    end
    
    AiEnable = State.create()
    AiEnable.onEnter = function(self)
   
         self:SetVar("AiDisabled", false)
        
         SetResetState(self) 
        
    end   
    
   --setState("Dead", self) 

end    
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Death
    Dead = State.create()

Dead.onEnter = function(self)
    
    
        self:SetVar("CurrentState", "Dead")
        if self:GetVar("I_Have_A_Parent") then
            local WpSetName = self:GetVar("attached_path")
            local  myParent = getParent(self)
            local sSplit =  split(WpSetName,"_")
            local xSplit = {}
            local xSplit= {alpa=sSplit[1] , num=sSplit[2] }
            myParent:SetVar( self:GetVar("SpawnedVar") , "NotSpawned") 
            setState("DeadChild", myParent) 
        end

       
        
        if self:GetVar("Im_A_Child") then
              myParent = getParent(self)
             for i = 1, getParent(self):GetVar("MaxPet") do
                 local myidString = self:GetID()
                 local myfinalID = "|" .. myidString
                 local Pet = getPetID(myParent,i):GetID()

                if  Pet == self:GetID() then
                     myParent:SetVar("StoredPet."..i, "none") 
                     myParent:SetVar("Child_Timer."..i, myfinalID )   
                     GAMEOBJ:GetTimer():AddTimerWithCancel( myParent:GetVar("Set.Pet_Respawn") , myfinalID, myParent )
                    
                end
             end

        end          


  
  
      
         getVarables(self) 
         CancelTemplateTimers(self)
      
        
end
    
    -- //////////////////////////////////////////////////////////////////////////////////
    -- Tether
    tether = State.create()
tether.onEnter = function(self)
        self:SetVar("aggrotarget",2)
        self:SetImmunity{ immunity = true }
        self:SetVar("myTarget", nil)
        if self:GetVar("Set.tetherSpeed") ~= nil then
                local Tspeed =  self:GetVar("Set.tetherSpeed")
        else
            local Tspeed = 1
        end
      
        --print("I am tethering") 
                  local myPos = getHomePoint(self)
                    self:GoTo { speed = self:GetVar("Set.tetherSpeed"),
                                target = { x = myPos.x,
                                           z = myPos.z,
                                           y = myPos.y,
                                         }
                                }
end
        

tether.onArrived = function(self,msg)

                self:SetVar("CurrentState", "tether")
             -- print("I have arrived at my home tether point.")
                self:SetVar("aggrotarget",0)
              self:SetHealth{ health = self:GetMaxHealth{}.health }
              self:SetImmunity{ immunity = false }
              getVarables(self)
     
              SetResetState(self)  --  Find what state i should reset too 
           
end

        GoHome = State.create()

GoHome.onEnter = function(self)
             self:SetVar("CurrentState", "GoHome")
            local hpos = getHomePoint(self)

            


            --if self:GetVar("Set.FearHP") then
                -- self:SetVar("FleeStatus",0)
                -- self:SetVar("FleeDone",true)
                -- self:SetVar("FleeRetrun",true)          
                -- setState("aggro", self)
            --else
                 self:GoTo { speed = 1, target = { x =hpos.x ,z =hpos.z ,y =hpos.y } }
           -- end
end
        
GoHome.onArrived = function(self)
        -- print("I have Arrived at my Home Point")
        self:SetRotation(self:GetVar("rot")) 
        SetResetState(self)  
end
     
-- //////////////////////////////////////////////////////////////////////////////////
-- Aggro 
function dist(a ,b)
   local dx = math.abs(b.x - a.x)
   local dy = math.abs(b.y - a.y)
   local dz = math.abs(b.z - a.z)
   local d = math.sqrt((dx ^ 2) + (dy ^ 2) + (dz ^ 2))
   return d
end 
function Distance(point1, point2)

   local distance = { }
   distance.x = (point2.x - point1.x)
   distance.y = (point2.y - point1.y)

   return distance

end
--[[
	Aggro::
	
	Right away, turn on the heartbeat timer and follow the target.  The FollowTarget method will continually follow, face the target, and keep a good attacking distance.
	When the heartbeat comes back, decide what to do.  For now, we just say that we are ready to attack.
	If you are a given distance from the target and you are facing him, attack.  If you cannot attack, wait until onArrived and then attack.
	
--]]

function AggroState(self)

    aggro = State.create()
    
    -----------------------------------------------------------------------------
    -- Aggro onEnter
    -----------------------------------------------------------------------------
    aggro.onEnter = function(self)
           
         self:SetVar("CurrentState", "aggro")
        local INITIAL_ATTACK_TIME = 1
        -- Start the heartbeat
        self:SetVar("readyToAttack", false)
        GAMEOBJ:GetTimer():AddTimerWithCancel(  INITIAL_ATTACK_TIME , "AttackHeartBeat", self )
        GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )

      
        local myPos = getHomePoint(self)
        local target = getObjectByName(self, "AttackingTarget")
        
        if  target:IsDead().bDead  then
            self:SetVar("Aggroed", false)
            ProximityPuls(self)    
        else
            if self:GetVar("Set.MovementType") ~= "Frozen" then
                self:SetTetherPoint { tetherPt = myPos,radius = self:GetVar("Set.tetherRadius") }
                -- print("Setting Tether Point ")  
                if self:GetVar("Set.AggroDist") ~= nil and self:GetVar("Set.AggroSpeed") ~= nil then 
                    self:FollowTarget { targetID = target, radius = self:GetVar("Set.AggroDist"),speed = self:GetVar("Set.AggroSpeed") * 2 , keepFollowing = true }
                     self:SetVar("Aggroed", true)
                else
                    self:FollowTarget { targetID = target,radius = 3, speed = 1, keepFollowing = true }
                   
                end
            end -- end if frozen
      
   
      end -- end else is not dead
    end -- end function
    
    
    aggro.onTimerDone = function(self, msg)
          if msg.name == "AttackHeartBeat" then
            self:SetVar("readyToAttack", true)
            DoAttack(self);
                        local castTime = self:GetVar("SkillTime")
            if (castTime < 1) then
                print("castTime", castTime)    
            end
            GAMEOBJ:GetTimer():AddTimerWithCancel(  castTime , "AttackHeartBeat", self )
        else
            onTimerDone(self, msg)
        end
    end
    
    aggro.onArrived = function(self, msg)
           
            DoAttack(self)
    end
    
    aggro.onExit = function(self)
        GAMEOBJ:GetTimer():CancelTimer("AttackHeartBeat", self)
    end
  
    -- This will be called if FollowTarget fails.  That would only fail because the target has died or logged out.  In any case, the target was lost
    aggro.onCancelled = function(self, msg)
          self:SetVar("Aggroed", false)
        --setState("tether",self)
          ProximityPuls(self)    
    end
					
    function DoAttack(self)
       
        if (self:GetVar("readyToAttack") == false) then
           -- print("Not Ready to Attack") 
            return
        end
        
        if CanAttackTarget(self) then
            -- print("Can Attack") 
            self:SetVar("readyToAttack", false)
            if self:GetVar("Set.Aggression") == "PassiveAggres" and self:GetVar("AggroOnce") == 0 then
                self:SetVar("AggroOnce",1)
            end
            if self:GetVar("MaxTableSkill") == nil then
             
                for i = 1, table.maxn (self:GetSkills().skills) do  
                    self:SetVar("MaxTableSkill", i ) 
                end 
                
            end 
 
        -- set current skill to max entry in table (default behavior)
        local attackSkillID = self:GetSkills().skills[self:GetVar("MaxTableSkill")]
        
        -- look for an attack override skill and use it
        local bOverrideSkill = self:GetVar("Set.OverRideAttackSkill")
        local overrideSkillID = self:GetVar("Set.AttackSkill")
        if (bOverrideSkill and overrideSkillID and bOverrideSkill == true) then
            attackSkillID = overrideSkillID
        end
        
            -- use an optional target if needed
            local bUseOptTarget = self:GetVar("Set.UseOptionalTargetOnAttack")
            if (bUseOptTarget and bUseOptTarget == true) then
              
                self:CastSkill{skillID = attackSkillID, optionalTargetID = getObjectByName(self, "AttackingTarget") } 
            else
                self:CastSkill{skillID = attackSkillID } 
            end
        
        end
    end
    
   
end -- end create aggro state

-----------------------------------------------------------------------------
-- Combat state - native code handling it
-----------------------------------------------------------------------------
function CombatState(self)
	combat = State.create()
	
	combat.onEnter = function(self)
		GAMEOBJ:GetTimer():CancelTimer( "MeanderPause", self )
		self:SetVar("CurrentState", "combat")
	end -- function combat.onEnter
end

 -----------------------------------------------------------------------------
    -- CanAttackTarget
    -----------------------------------------------------------------------------
function CanAttackTarget(self)
        local myTarget = getObjectByName(self, "AttackingTarget")
        --local range = self:GetVar("Set.AggroDist")
        --local myPos = Vector.new(self:GetPosition().pos)
        --local hisPos = Vector.new(myTarget:GetPosition().pos)
        --local dist = hisPos - myPos
        
       if not myTarget:Exists() or myTarget:IsDead().bDead  or not self:IsEnemy{ targetID = myTarget }.enemy then
          self:SetVar("AttackingTarget", "NoTarget" )
          self:SetVar("Aggroed", false)
          ProximityPuls(self)
          return false
       end
       
        -- If we are close enough and if the object is in the FOV, then attack
        --if dist:sqrLength() - range <= range * range then --and (self:IsObjectInFOV { target = myTarget, radius = 5 , minRange = 0, maxRange = 100 }.result) then
                return true
        --end
        --self:SetVar("Aggroed", false)
        --return false
        
end -- end CanAttackTarget


-- Start timer funciton
function SetStoreTimmer(intTime, Name, self) 

	
	-- Start Timer
	if 	(intTime) and (Name) and (self) then
	    GAMEOBJ:GetTimer():AddTimerWithCancel( intTime,  Name, self )
	end
	
	-- Store Timer
	 for  i = 1, table.maxn(self:GetVar("Timers")) + 1 do
	    if self:GetVar("Timers."..i) == Name then
	        break
	    end
	 	if self:GetVar("Timers."..i) == nil then
	 		 self:SetVar("Timers."..i, Name)
	 	end 
     end
     
end 

function CancelTemplateTimers(self)

	if self:GetVar("Timers") then
	
	  for i = 2,table.maxn(self:GetVar("Timers")) do
			GAMEOBJ:GetTimer():CancelTimer( self:GetVar("Timers."..i), self )
	  end
	  
   end 
end


onDie = function(self,msg)

    if (onTemplateDie) and (onTemplateDie(self, msg) == true ) then
    	return
    	
    end 
    
end

