-----------------------------------------------------------------
--Spider Boss 1 script
-----------------------------------------------------------------

function onStartup(self)
    
    self:AddObjectToGroup{group = "SpiderBossPhase1"}
    self:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
    self:SetVar("HitTime", 1.8)
    self:SetVar("StunTime", 15)
    self:SetVar("AttackModDelay", 20)
    self:SetVar("AttackModTime", 5)
    --self:SetVar("AttackingModules", false)
    
    local group = self:GetObjectsInGroup{group = "BossInteracts", ignoreSpawners = true}.objects
   
    for i, object in pairs(group) do
   
        if object then
            --spider doesn't hate the coils here--
            self:AddThreatRating{newThreatObjID = object, ThreatToAdd = -1000}
        end
    end
   
    GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModDelay"), "AttackDelay", self)
end



function onNotifyObject(self, msg)
   
    if msg.name == "Boom" then
        --print("Spider BOOM!")
        self:PlayAnimation{animationID = "spider-laser"}
        self:ActivityTimerStopAllTimers()
        self:TimerCancelled{name = "AttackDelay"}
        self:TimerCancelled{name = "AttackTime"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("HitTime")  , "HitTime", self )
    end
end



function onTimerDone(self, msg)
   
    if msg.name == "AttackDelay" then
        --self:SetVar("AttackingModules", true)
        --print("spider attacking the modules!")
      
        local group = self:GetObjectsInGroup{group = "BossInteracts", ignoreSpawners = true}.objects
      
        for i, object in pairs(group) do
         
            if object then
                --spider HATES the coils here--
                self:AddThreatRating{newThreatObjID = object, ThreatToAdd = 2000}
            end
        end
      
        GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModTime"), "AttackTime", self)
   
    elseif msg.name == "AttackTime" then
        --self:SetVar("AttackingModules", false)
        --print("spider attacking the player!")
      
        local group = self:GetObjectsInGroup{group = "BossInteracts", ignoreSpawners = true}.objects
      
        for i, object in pairs(group) do
         
            if object then
                --spider doesn't hate the coils here--
                self:AddThreatRating{newThreatObjID = object, ThreatToAdd = -2000}
            end
        end
      
        GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModDelay"), "AttackDelay", self)
      
    elseif msg.name == "HitTime" then
        --print("the spider is stunned...  ATTACK!")
      
        self:SetFaction{faction = 110}
        self:PlayAnimation{animationID = "spider-stun"}
        GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("StunTime"), "StunFinished", self)
        self:ClearThreatList()
      
    elseif msg.name == "StunFinished" then
        --print("the spider isn't stunned... RUN!")
        
        self:SetFaction{faction = 38}
        self:PlayAnimation{animationID = "idle"}
        
        local group = self:GetObjectsInGroup{group = "BossInteracts", ignoreSpawners = true}.objects
        
        self:ClearThreatList()
   
        for i, object in pairs(group) do
   
            if object then
                --spider doesn't hate the coils here--
                self:AddThreatRating{newThreatObjID = object, ThreatToAdd = -1000}
            end
        end
   
        GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModDelay"), "AttackDelay", self)
      
        --[[if self:GetVar("AttackingModules") == true then
            GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModTime"), "AttackTime", self)
         
        else
            GAMEOBJ:GetTimer():AddTimerWithCancel(self:GetVar("AttackModDelay"), "AttackDelay", self)
        end--]]
    end
end




function onOnHit(self, msg)
   
    self:PlayAnimation{animationID = "spider-stun"}
end




function onDie(self, msg)

    local object = self:GetObjectsInGroup{group = "Coil", ignoreSpawners = true}.objects[1]
    
    if object then
        object:PlayFXEffect{name = "bigboomsupercharge", effecdID = 580, effectType = "create"}
        object:Die()
        print("destroying the coil")
    end
    
    local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
      
    for i, object in pairs(group) do
        object:NotifyObject{name = "CoilDestroyed", ObjIDSender = self}
        print("telling the switch that the coil is dead")
    end
    
    local group = self:GetObjectsInGroup{group = "BossPipes", ignoreSpawners = true}.objects
      
    for i, object in pairs(group) do
        object:Die()
    end
    
    local spawnerObj = LEVEL:GetSpawnerByName("BossTurBase")
   
    if spawnerObj then
        spawnerObj:SpawnerActivate()
    end
    
    local spawnerObj = LEVEL:GetSpawnerByName("BossTurret")
   
    if spawnerObj then
        spawnerObj:SpawnerActivate()
    end
    
    local spawnerObj = LEVEL:GetSpawnerByName("SpiderBoss3")
   
    if spawnerObj then
        spawnerObj:SpawnerActivate()
    end
end