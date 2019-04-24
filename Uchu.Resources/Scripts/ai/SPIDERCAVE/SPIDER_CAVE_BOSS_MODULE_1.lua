-------------------------------------------------------------------------------
--script for first boss rebuild module
-------------------------------------------------------------------------------

function onStartup(self)
    self:SetVar("SpiderSpawned", false)
    self:SetVar("CoilSmashDown", false)
    --self:SetVar("Mod2SmashDown", false)
end

function onRebuildNotifyState(self, msg)
    --print("the rebuild state is: "..msg.iState)
    if msg.iState == 2 then
        local object = self:GetObjectsInGroup{group = "FinalModule", ignoreSpawners = true}.objects[1]
        if object then
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
        end
        if self:GetVar("CoilSmashDown") == true then
            print("module 1 talking to coil module QB")
            local object2 = self:GetObjectsInGroup{group = "CoilModule", ignoreSpawners = true}.objects[1]
            if object2 then
                object2:NotifyObject{name = "ModuleOneUp", ObjIDSender = self}
            end
        elseif self:GetVar("CoilSmashDown") == false then
            print("module 1 talking to coil module smashable")
            local object2 = self:GetObjectsInGroup{group = "CoilModSmash", ignoreSpawners = true}.objects[1]
            if object2 then
                object2:NotifyObject{name = "ModuleOneUp", ObjIDSender = self}
            end
        end
        if self:GetVar("SpiderSpawned") == false then
            self:SetVar("SpiderSpawned", true)
            local spawnerObj = LEVEL:GetSpawnerByName("SpiderBoss2")
            if spawnerObj then
                print("spawning spider boss!")
                spawnerObj:SpawnerActivate()
                --spawnerObj:SpawnerDeactivate()
            end
        end
    elseif msg.iState == 4 then
        print("module 1 destroyed!")
        if self:GetVar("CoilSmashDown") == true then
            local object = self:GetObjectsInGroup{group = "CoilModule", ignoreSpawners = true}.objects[1]
            if object then
                object:NotifyObject{name = "ModuleOneDown", ObjIDSender = self}
            end
        else
            local object = self:GetObjectsInGroup{group = "CoilModSmash", ignoreSpawners = true}.objects[1]
            if object then
                object:NotifyObject{name = "ModuleOneDown", ObjIDSender = self}
            end
        end
        local object = self:GetObjectsInGroup{group = "FinalModule", ignoreSpawners = true}.objects[1]
        if object then
            object:StopFXEffect{name = "moviespotlight"}
        end
    end
end

function onNotifyObject(self, msg)
    if msg.name == "ModuleCoilSmashDown" then
        print("module 1 knows that the coil module was smashed")
        self:SetVar("CoilSmashDown", true)
    end
end