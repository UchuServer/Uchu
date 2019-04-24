function onStartup(self)
    --print("starting up the switch")
    self:SetVar("finalDestination", false)
    --self:SetVar("firstDestination", false)
end

function onNotifyObject(self, msg)
    --print("the switch got a message")
    if (msg.name == "arrivedAtFinalWayPoint") then
        self:SetVar("finalDestination", true)
    end
end

function onFireEvent(self, msg)
    --print("switch activated")
    local friends = self:GetObjectsInGroup{group = "SpiderRailTurret", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6528) then
            object:NotifyObject{name = "fire", ObjIDSender = self}
        end
    end
    if (self:GetVar("finalDestination") == true) then
        --local friends = self:GetObjectsInGroup{group = "SpiderRailTurret", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 5652) then
                object:ToggleTrigger{enable = false}
            elseif (object:GetLOT().objtemplate == 6457) then
                object:NotifyObject{name  = "shocked", ObjIDSender = self}
            end
        end
    end
end