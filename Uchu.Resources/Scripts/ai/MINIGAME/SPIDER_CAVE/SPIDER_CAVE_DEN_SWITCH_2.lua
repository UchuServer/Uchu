function onStartup(self)
    self:AddObjectToGroup{group = "DenSwitch"}
    --print("starting up switch 2")
    self:SetVar("switch1", false)
    self:SetVar("switch2", false)
    self:SetVar("TickTime", 30)
end

function onFireEvent(self, msg)
    print("switch 2 activated")
    self:SetVar("switch2", true)
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "TickTime", self )
    local friends = self:GetObjectsInGroup{group = "DenSwitch", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6540) then
            object:NotifyObject{name = "switch2Activated", ObjIDSender = self}
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
   print("switch 2 got a message")
   if (msg.name == "switch1Activated") then
      self:SetVar("switch1", true)
   elseif (msg.name == "switch1Deactivated") then
      self:SetVar("switch1", false)
   end
end

function onTimerDone(self, msg)
    print("switch 2 deactivated")
    self:SetVar("switch2", false)
    local friends = self:GetObjectsInGroup{group = "DenSwitch", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6540) then
            object:NotifyObject{name = "switch2Deactivated", ObjIDSender = self}
        end
    end
end