------------------------------------------------------
-- Generic Script to attach to the server side of a
-- NPC so that it will follow paths and wander around.
-- 
-- created 7/06/10 btarr... 
------------------------------------------------------

function onStartup(self)
    self:SetVar("Set.SuspendLuaMovementAI", true)   -- a state suspending scripted movement AI
    
    self:SetVar("Set.MovementType", "Wander")       -- this is how the NPC will behave when not on a path. 
    -- Wander Settings ---------------------------------------------------------
    self:SetVar("Set.WanderChance",100)             -- Main Weight
    self:SetVar("Set.WanderDelayMin",5)             -- Min Wander Delay
    self:SetVar("Set.WanderDelayMax", 5)            -- Max Wander Delay
    self:SetVar("Set.WanderSpeed",1)                -- Move speed 
    self:SetVar("Set.wanderRadius",30)              -- Move radius 
end 