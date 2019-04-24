require('o_mis')
require('State')

function onStartup(self)


     --###########################################################
     --########## Set Variables 
     --###########################################################
     self:SetVar("REBUILD_STATE", "null")
     
     self:SetProximityRadius { radius = 20 , name = "aggroRadius" }
     self:SetProximityRadius { radius = 50 , name = "resetRadius" }
     storeHomePoint(self)
     -- Start Break timer -- 
     GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "BreakStart",self )	
     GAMEOBJ:GetTimer():AddTimerWithCancel( 8, "BreakBase",self )	
     local pos = getHomePoint(self)
     RESMGR:LoadObject { objectTemplate =  3086 , x= pos.x   , y= pos.y , z= pos.z  , owner = self } -- Student 1
     
     self:UseStateMachine{} 
     
     --###########################################################
     --########## STATES
     --###########################################################
     
     
      Idle = State.create()
      Idle.onEnter = function(self)
           
            
      end 
      Idle.onArrived = function(self)
             
      end  

    --###########################################################
    --########## TETHER
    --###########################################################
    tether = State.create()
    tether.onEnter = function(self)
       -- Rebuild Reset -- 
       self:RebuildReset()
    end    
    tether.onArrived = function(self,msg)

    end

    --###########################################################
    --########## AGGRO
    --###########################################################   
    aggro = State.create()
    -- Aggro onEnter
    aggro.onEnter = function(self)
        local INITIAL_ATTACK_TIME = 1
        -- Start the heartbeat
        self:SetVar("readyToAttack", false)
        GAMEOBJ:GetTimer():AddTimerWithCancel(  INITIAL_ATTACK_TIME , "AttackHeartBeat", self )
              
        local myPos = getHomePoint(self)
        local target = getMyTarget(self)
        
        if  target:IsDead().bDead  then
            setState("tether",self)     
        else
            self:SetTetherPoint { tetherPt = myPos,radius = 60 }
           -- self:FollowTarget { targetID = target, radius = 15 ,speed = 0 , keepFollowing = true }   
           self:FaceTarget{ target = getMyTarget(self), degreesOff = 5, keepFacingTarget = true }
 
        end
            
      
    end -- end function
    
    
    aggro.onTimerDone = function(self, msg)
          if msg.name == "AttackHeartBeat" then
            self:SetVar("readyToAttack", true)
            DoAttack(self);
            local castTime = 1
           
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
        setState("tether",self)
    end
					
    function DoAttack(self)
        if(self:GetVar("readyToAttack") == false) then
            return
        end
        
        if CanAttackTarget(self) then
            self:SetVar("readyToAttack", false)
            
            if self:GetVar("MaxTableSkill") == nil then
                for i = 1, table.maxn (self:GetSkills().skills) do  
                    self:SetVar("MaxTableSkill", i ) 
                end 
                
            end 
 
                self:CastSkill{skillID = self:GetSkills().skills[self:GetVar("MaxTableSkill")] } 
        
        end
    end
    
      addState(Idle, "Idle", "Idle", self)
      addState(tether, "tether", "tether", self)
      addState(aggro, "aggro", "aggro", self)
      beginStateMachine("Idle", self) 

end
    --###########################################################
    --########## CAN ATTACK
    --###########################################################
function CanAttackTarget(self)
    local myTarget = getMyTarget(self)
    local range = 12
    local myPos = Vector.new(self:GetPosition().pos)
    local hisPos = Vector.new(myTarget:GetPosition().pos)
    local dist = hisPos - myPos
    
   if not myTarget:Exists() or myTarget:IsDead().bDead then
      setState("tether",self)
      return false
   end
   
   if self:GetVar("REBUILD_STATE") == "broke" then
     return false
   end
   
    -- If we are close enough and if the object is in the FOV, then attack
    if dist:sqrLength() <= range * range then --and (self:IsObjectInFOV { target = myTarget, radius = 5 , minRange = 0, maxRange = 100 }.result) then
            return true
    end
    return false
    
end -- end CanAttackTarget 
    --###########################################################
    --########## PROXIMITY
    --###########################################################   
    

     
     
function onProximityUpdate(self, msg)
      if msg.objType == "Enemies" or msg.objType == "NPC" or msg.objType == "Rebuildables" and  msg.objId:GetFaction().faction == 1 and  self:GetVar("REBUILD_STATE") ~= "broke" and msg.status == "ENTER" and msg.name == "aggroRadius" then
        storeTarget(self, msg.objId)
        setState("aggro", self) 
      end 
      if msg.objType == "Enemies" or msg.objType == "NPC" or msg.objType == "Rebuildables" and  msg.objId:GetFaction().faction == 1 and  self:GetVar("REBUILD_STATE") ~= "broke" and msg.status == "LEAVE" and msg.name == "resetRadius" then
         local pet = getPet(self, "Child" , 1 )
         pet:Die{ killType = "SILENT" }
          GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "BreakStart",self  )
      end       
      
end
    --###########################################################
    --########## REBUILD NOTIFY
    --###########################################################
function onRebuildNotifyState(self, msg)
	if (msg.iState) == 2 then
	 GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "SpawnBase",self )	
	 self:SetVar("REBUILD_STATE", "null")
	end
	if (msg.iState) == 1 then 
	     local targets = msg.player
	     storeTarget(self, targets)
	     self:FaceTarget{ target = targets, degreesOff = 5, keepFacingTarget = true }
	     setState("aggro", self)
	end
	if (msg.iState) == 4 then -- Cancle
	     self:SetImmunity{ immunity = true }
	     self:SetVar("REBUILD_STATE", "broke")
	end
end   
    --###########################################################
    --########## ON HIT
    --###########################################################
function onOnHit(self,msg)   
   hp = self:GetHealth{}.health
    print("health ="..hp)
    if hp < 7 then
     storeTarget(self, msg.attacker)
     GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "BreakStart",self )	
 
     local pet = getPet(self, "Child" , 1 )
     pet:Die{ killType = "SILENT"}
     setState("aggro", self) 
    end 
end
    --###########################################################
    --########## CHILD LOADED
    --###########################################################
onChildLoaded = function(self,msg)
       
         storePet(self, msg.childID, "Child" , 1 )
                
end 

    --###########################################################
    --########## TIMER
    --###########################################################
onTimerDone = function(self, msg)

    if msg.name == "BreakStart" then 
         self:SetVar("REBUILD_STATE", "broke")
         self:RebuildReset()
         self:SetImmunity{ immunity = true }
         self:SetHealth{ health = 10 }

    end
    if msg.name == "BreakBase" then
         local pet = getPet(self, "Child" , 1 )
         pet:Die{ killType = "SILENT" }
          
    end
    
    if msg.name == "SpawnBase" then
       local pos = getHomePoint(self)
       RESMGR:LoadObject { objectTemplate =  3086 , x= pos.x   , y= pos.y , z= pos.z  , owner = self } -- Student 1
       self:SetImmunity{ immunity = false }
    end  

end 