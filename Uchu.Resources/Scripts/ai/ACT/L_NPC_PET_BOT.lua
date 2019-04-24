require('State')
require('Vector')
local ATTACK_DIST = 5;
local RUN_SPEED = 4;
function onStartup(self)
    local AGGRO_RAD           = 50;
    local MAX_PARENT_DIST     = 200;
    
    self:AddSkill{skillID = 55}
    storeObj(self, nil, "myTarget");
    
    print("Your pet is ready")
    print("ParentID = ", self:GetVar("parentID"))
    
    self:SetProximityRadius { radius = AGGRO_RAD, name = "aggro" };
    self:SetProximityRadius { radius = MAX_PARENT_DIST, name = "parentRad" };
 
    self:UseStateMachine();
    addState(FollowParent, "FollowParent", "FollowParent", self);
    addState(Aggro, "Aggro", "Aggro", self);
    addState(CatchUp, "CatchUp", "CatchUp", self);   
    beginStateMachine( "FollowParent", self );
end

--------------------------------------------------------
-- PUT THIS IN A UTIL SCRIPT
--------------------------------------------------------
function storeObj(self, target, name)
	if(target) then
        idString = target:GetID();
	    finalID = "|" .. idString;
	    self:SetVar(name, finalID);
	else
	    self:SetVar(name, nil);
	end
end

function getObj(self, name)
	targetID = self:GetVar(name);
	if(targetID) then
	    return GAMEOBJ:GetObjectByID(targetID);
	else
	    return nil;
	end
end


------------------------------------------------------------------------------------------------------------
-- Follow Parent
------------------------------------------------------------------------------------------------------------
FollowParent = State.create();

FollowParent.onEnter = function(self)
    self:PetFollowOwner{ bFollowState = true } 
end

FollowParent.onExit = function(self)
    self:PetFollowOwner{ bFollowState = false } 
end

------------------------------------------------------------------------------------------------------------
-- Aggro
------------------------------------------------------------------------------------------------------------
Aggro = State.create();

-----------------------------------------------------------------------------
-- onEnter
-----------------------------------------------------------------------------
Aggro.onEnter = function(self)
    local INITIAL_ATTACK_TIME = .5;
 
   local parentObj = getObj(self, "parentID");
   parentObj:PetFollowOwner{ bFollowState = false } 
    
    
    -- Start the heartbeat
    self:SetVar("readyToAttack", false);
    GAMEOBJ:GetTimer():AddTimerWithCancel(  INITIAL_ATTACK_TIME , "AttackHeartBeat", self );
        
    local target = getObj(self, "myTarget");
    if  target:IsDead().bDead  then
        setState("FollowParent",self);
    else
        self:FollowTarget { targetID = target,ATTACK_DIST, speed = RUN_SPEED, keepFollowing = true };
    end

end -- end function

-----------------------------------------------------------------------------
-- onExit
-----------------------------------------------------------------------------
Aggro.onExit = function(self)
    GAMEOBJ:GetTimer():CancelTimer("AttackHeartBeat", self);
    storeObj(self, nil, "myTarget");
--    parentObj:PetFollowOwner{ bFollowState = true } 
end
    
    
-----------------------------------------------------------------------------
-- Aggro update
-----------------------------------------------------------------------------
Aggro.onTimerDone = function(self, msg)
    if msg.name == "AttackHeartBeat" then
        self:SetVar("readyToAttack", true);
        DoAttack(self);
        local castTime = 1.5;
        GAMEOBJ:GetTimer():AddTimerWithCancel(  castTime , "AttackHeartBeat", self );
    end
end



-----------------------------------------------------------------------------
-- onArrived
-----------------------------------------------------------------------------
Aggro.onArrived = function(self, msg)
    DoAttack(self);
end
    
-- This will be called if FollowTarget fails.  That would only fail because the target has died,
Aggro.onCancelled = function(self, msg)
    setState("FollowParent",self);
end


-----------------------------------------------------------------------------
-- onArrived
-----------------------------------------------------------------------------
function DoAttack(self)
    if(self:GetVar("readyToAttack") == false) then
        return
    end
    
    if CanAttackTarget(self) then
        self:SetVar("readyToAttack", false)
        self:PlayFXEffect{effectType = "run"}
        self:CastSkill{skillID = 55 }   -- TODO: This should be defined somewhere easier to find
        
    end
end


function CanAttackTarget(self)
        local myTarget = getObj(self, "myTarget");
        local range = ATTACK_DIST;
        local myPos = Vector.new(self:GetPosition().pos)
        local hisPos = Vector.new(myTarget:GetPosition().pos)
        local dist = hisPos - myPos
        
       if not myTarget:Exists() or myTarget:IsDead().bDead then
          setState("FollowParent",self)
          return false
       end
       
        -- If we are close enough and if the object is in the FOV, then attack
        if dist:sqrLength() <= range * range then --and (self:IsObjectInFOV { target = myTarget, radius = 5 , minRange = 0, maxRange = 100 }.result) then
                return true
        end
        return false
        
    end -- end CanAttackTarget
    
    
------------------------------------------------------------------------------------------------------------
-- Catch up to parent
------------------------------------------------------------------------------------------------------------
CatchUp = State.create();

CatchUp.onEnter = function(self)
    self:PetFollowOwner{ bFollowState = true } 
end

CatchUp.onProximityUpdate = function(self, msg)
    -- ENTER
    -- if the object is the owner, go to FollowParent
    
    
    if(msg.name == "parentRad") then
        if  (msg.status == "ENTER" and msg.objId:GetID() == getObj(self, "parentID"):GetID()) then
        setState("FollowParent", self)
        end    
    end
end

------------------------------------------------------------------------------------------------------------
-- Global functions
------------------------------------------------------------------------------------------------------------
function onProximityUpdate(self, msg)

    if(msg.name == "aggro") then
        if getObj(self, "myTarget") then
            return
        end
        
        if(msg.status == "ENTER") and self:IsEnemy{ targetID = msg.objId }.enemy == true and not msg.objId:IsDead().bDead then
            storeObj(self, msg.objId, "myTarget");
            setState("Aggro", self)
	    end   
    else 
    -- ENTER
    -- check if object is an enemy and attack.
        if(msg.status == "LEAVE") then
             -- if msg.status == "LEAVE"
            if(msg.objId:GetID() == getObj(self, "parentID"):GetID()) then
             setState("CatchUp", self);
            end
         end
    end
end


