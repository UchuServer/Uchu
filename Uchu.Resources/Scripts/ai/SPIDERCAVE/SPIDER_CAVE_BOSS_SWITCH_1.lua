----------------------------------------------------
--switch that fires the coil/turret once all the modules are built
----------------------------------------------------

local HitTime = 1.8

function onStartup(self)
    self:SetVar("AmActive", false)
    self:SetVar("Switch2Active", false)
    self:SetVar("AmPowered", false)
    self:SetVar("CoilSmashDown", false)
    self:SetVar("Mod2SmashDown", false)
    self:SetVar("CoilDown", false)
    self:SetVar("TurretUp", false)
    self:SetVar("TurretBaseUp", false)
    self:SetVar("FiredOnce", false)
end



function onFireEvent(self, msg)
    
    if msg.args == "down" then
        --print("pressed down")
        if self:GetVar("CoilDown") == true and self:GetVar("TurretUp") == true and self:GetVar("TurretBaseUp") == true and self:GetVar("AmPowered") == true and self:GetVar("FiredOnce") == false then
            print("everything activated")
            
            self:SetVar("FiredOnce", true)
            
            ---------------------------------------------------------------------------
            --fire the super turret--
            ---------------------------------------------------------------------------
            
            local object = self:GetObjectsInGroup{group = "Turret", ignoreSpawners = true}.objects[1]
            
            if object then
                object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            end
            
            ---------------------------------------------------------------------------
            --play spider "stunned" animation--
            ---------------------------------------------------------------------------
            
            local object = self:GetObjectsInGroup{group = "SpiderBossPhase2", ignoreSpawners = true}.objects[1]
            
            if object then
                object:PlayAnimation{animationID = "spider-laser"}
                object:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
                GAMEOBJ:GetTimer():AddTimerWithCancel(HitTime, "HitTime", self )
            end
        
        elseif self:GetVar("AmPowered") == true and self:GetVar("CoilDown") == false then
            
            ---------------------------------------------------------------------------
            --pick a random module to smash--
            ---------------------------------------------------------------------------
            
            local BreakThisModule = math.random(3)
            local Module1 = 1
            local Module2 = 2
            local Module3 = 3
            
            print("smashing module: " .. BreakThisModule)
            
            ---------------------------------------------------------------------------
            --play the effect on the coil--
            ---------------------------------------------------------------------------
            
            local object = self:GetObjectsInGroup{group = "Coil", ignoreSpawners = true}.objects[1]
            
            if object then
                --print("playing effect on the coil")
                object:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
            end
            
            ---------------------------------------------------------------------------
            --figure out which module to smash--
            ---------------------------------------------------------------------------
            
            if BreakThisModule == Module1 then
                
                local object = self:GetObjectsInGroup{group = "BossQBMod1", ignoreSpawners = true}.objects[1]
                
                if object then
                    object:Die()
                end
            
            elseif BreakThisModule == Module2 then
            
                if self:GetVar("Mod2SmashDown") == false then
                    
                    local object = self:GetObjectsInGroup{group = "FinalModSmash2", ignoreSpawners = true}.objects[1]
                    
                    if object then
                        object:Die()
                    end
                
                elseif self:GetVar("Mod2SmashDown") == true then
                
                    local object = self:GetObjectsInGroup{group = "FinalModule2", ignoreSpawners = true}.objects[1]
                    
                    if object then
                        object:Die()
                    end
                end
                    
            elseif BreakThisModule == Module3 then
            
                if self:GetVar("CoilSmashDown") == false then
                    
                    local object = self:GetObjectsInGroup{group = "CoilModSmash", ignoreSpawners = true}.objects[1]
                    
                    if object then
                        object:Die()
                    end
                
                elseif self:GetVar("CoilSmashDown") == true then
                
                    local object = self:GetObjectsInGroup{group = "CoilModule", ignoreSpawners = true}.objects[1]
                    
                    if object then
                        object:Die()
                    end
                end
            end
            
            ---------------------------------------------------------------------------
            --telling the spider boss that it should be stunned--
            ---------------------------------------------------------------------------
            
            local object = self:GetObjectsInGroup{group = "SpiderBossPhase1", ignoreSpawners = true}.objects[1]
            
            if object then
                print("boom!")
                object:NotifyObject{name = "Boom"}
            end
        end
    end
end



function onTimerDone(self, msg)
    
    ----------------------------------------------------------------------
    --kill the spider!--
    ----------------------------------------------------------------------
    
    local object = self:GetObjectsInGroup{group = "SpiderBossPhase2", ignoreSpawners = true}.objects[1]
        
    if object then
        object:Die()
    end
    
    local object = self:GetObjectsInGroup{group = "Turret", ignoreSpawners = true}.objects[1]
        
    if object then
        object:StopFXEffect{name = "moviespotlight"}
    end
    
    local posString = self:CreatePositionString{ x = -612.46, y = -481.69, z = 110.97 }.string
    local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", true} }
    --print("spawning QB")
    RESMGR:LoadObject { objectTemplate = 6447, x= -599.35, y= -481.8 , z= 76.62, ry= 17.00, configData = config }

end



function onNotifyObject(self, msg)
   
    if msg.name == "AllUp" then
        self:SetVar("AmPowered", true)
        --print("the switch is powered!")
    
    elseif msg.name == "AllDown" then
        self:SetVar("AmPowered", false)
    
    elseif msg.name == "ModuleTwoSmashDown" then
        self:SetVar("Mod2SmashDown", true)
    
    elseif msg.name == "ModuleCoilSmashDown" then
        self:SetVar("CoilSmashDown", true)
    
    elseif msg.name == "CoilDestroyed" then
        self:SetVar("CoilDown", true)
        print("the coil is destroyed")
    
    elseif msg.name == "TurretBuilt" then
        self:SetVar("TurretUp", true)
        print("turret is built")
    
    elseif msg.name == "TurretBaseBuilt" then
        self:SetVar("TurretBaseUp", true)
        print("turret base is built")
    end
end