function onStartup(self)
    self:AddObjectToGroup{group = "DenSwitch"}
    --print("starting up switch 1")
    self:SetVar("switch1", false)
    self:SetVar("switch2", false)
    self:SetVar("TickTime", 30)
end

function onFireEvent(self, msg)
    print("switch 1 activated")
    self:SetVar("switch1", true)
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    local friends = self:GetObjectsInGroup{group = "DenSwitch", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6541) then
            print("telling switch 2")
            object:NotifyObject{name = "switch1Activated", ObjIDSender = self}
        end
    end
    if (self:GetVar("switch1") == true) and (self:GetVar("switch2") == true) then
        print("both switches activated")
        local friends = self:GetObjectsInGroup{group = "SpiderDen", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6537) then
                object:NotifyObject{name = "superTurretFired", ObjIDSender = self}
            end
        end
    end
end

function onNotifyObject(self, msg)
    print("switch 1 got a message")
   if (msg.name == "switch2Activated") then
      self:SetVar("switch2", true)
   elseif (msg.name == "switch2Deactivated") then
      self:SetVar("switch2", false)
   end
end

function onTimerDone(self, msg)
    print("switch 1 deactivated")
    self:SetVar("switch1", false)
    local friends = self:GetObjectsInGroup{group = "DenSwitch", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6541) then
            object:NotifyObject{name = "switch1Deactivated", ObjIDSender = self}
        end
    end
end