-------------------------------------------------------------
--Ronin script that kills them after a certain time
-------------------------------------------------------------


function onStartup(self)
    self:SetVar("Set.SuspendLuaMovementAI", true)   -- a state suspending scripted movement AI
    
    self:SetVar("Set.MovementType", "Wander")       -- this is how the NPC will behave when not on a path. 
    -- Wander Settings ---------------------------------------------------------
    self:SetVar("Set.WanderChance",100)             -- Main Weight
    self:SetVar("Set.WanderDelayMin",5)             -- Min Wander Delay
    self:SetVar("Set.WanderDelayMax", 5)            -- Max Wander Delay
    self:SetVar("Set.WanderSpeed",1)                -- Move speed 
    self:SetVar("Set.wanderRadius",5)               -- Move radius 


   

    self:AddObjectToGroup{group = "RoninEnemies"}
    
    -- add a 'suicide timer' if this object was spawned with a request for it
    -- (this normally happens when spawned by a statue, so that it is forced to reset back to statue mode after a bit)
    local suicideTimer = self:GetVar("suicideTimer")
    if suicideTimer and suicideTimer ~= 0 then
        GAMEOBJ:GetTimer():AddTimerWithCancel( suicideTimer, "Dead", self )
    end
    
end



function onTimerDone(self, msg)
    
    if msg.name == "Dead" then

        -- TODO: we need a message to request whether or not we have anyone on our threat list
        --       so that we can delay the timer again instead of killing ourselves... otherwise,
        --       it just looks like a bug...  (Though it has been pointed out that the statue is
        --       going to respawn regardless, so it's possible we don't want to actually do this...)
   
        self:RequestDie{killType = SILENT}
        
    end
end