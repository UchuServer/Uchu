----------------------------------------------------------
--This script teleports the player to a moving platform
----------------------------------------------------------

function onStartup(self)
   --print("teleporter starting up")
   self:SetVar("ModuleUp", false)
   self:SetVar("TeleportUp", false)
end

function onNotifyObject(self, msg)
    if msg.name == "ModuleBuilt" then
        self:SetVar("ModuleUp", true)
    elseif msg.name == "TeleportBuilt" then
        self:SetVar("TeleportUp", true)
    end
end

function onCollisionPhantom(self, msg)
   --print("teleporting player")
    if self:GetVar("ModuleUp") == true and self:GetVar("TeleportUp") == true then
        local object = self:GetObjectsInGroup{group = "teleport", ignoreSpawners = true}.objects[1]
        if object then
            local tele = object:GetPosition().pos
            local player = msg.objectID
            --local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
            --player:SetPosition {pos = tele}
            --print("setting position " .. tele.x  .. " " .. tele.y .. " " .. tele.z)
            --print("player ID" .. GAMEOBJ:GetLocalCharID())
            player:Teleport {pos = {x = tele.x + 4, y =  tele.y - 10, z = tele.z + 4}, bIgnoreY = false}
            object:PlayFXEffect{name = "febuildpop", effectID = 105, effectType = "create"}
        end
        --self:PlayFXEffect{name = "febuildpop", effectID = 105, effectType = "create"}
        self:PlayFXEffect{name = "ninjasmoke", effectID = 98, effectType = "create"}
    end
end

--player:Teleport {pos = x = tele.x , y =  tele.y - 10 , z = tele.z
--local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())