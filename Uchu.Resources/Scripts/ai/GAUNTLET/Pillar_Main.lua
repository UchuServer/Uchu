--------------------------------------------------------------
--Main Jail script for gauntlet level
--------------------------------------------------------------

function onStartup(self)
    self:AddObjectToGroup{name = "pillar"}
    --print("Jail starting up!")
    self:SetVar("Pillar_1_Destroyed", false)
    self:SetVar("Pillar_2_Destroyed", false)
    self:SetVar("Pillar_3_Destroyed", false)
    self:SetVar("Pillar_4_Destroyed", false)
end

function onNotifyObject(self, msg)
    --print("Jail got a message!")
    if (msg.name == "P_1_Destroyed") then
        --print("first pillar destroyed!")
        self:SetVar("Pillar_1_Destroyed", true)
    elseif (msg.name == "P_2_Destroyed") then
        self:SetVar("Pillar_2_Destroyed", true)
    elseif  (msg.name == "P_3_Destroyed") then
        self:SetVar("Pillar_3_Destroyed", true)
    elseif (msg.name == "P_4_Destroyed") then
        self:SetVar("Pillar_4_Destroyed", true)
    end
    
    if (self:GetVar("Pillar_1_Destroyed") == true) and (self:GetVar("Pillar_2_Destroyed") == true) and (self:GetVar("Pillar_3_Destroyed") == true) and (self:GetVar("Pillar_4_Destroyed") == true) then
        self:Die()
        --print("all pillars destroyed!")
        local spawnerObj = LEVEL:GetSpawnerByName("FinalSpawn")
        if spawnerObj then
            spawnerObj:SpawnerDeactivate()
            spawnerObj:SpawnerDestroyObjects()
        end
    end
end