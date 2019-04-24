--------------------------------------------------------
--Script on the module to tell the switch that it is built
--------------------------------------------------------

function onStartup(self)
   --print("starting up platform 2")
   self:SetVar("ModuleUp", false)
   self:SetVar("IAMbuilt", false)
end

function onNotifyObject(self, msg)
    if msg.name == "ModuleBuilt" then
        self:SetVar("ModuleUp", true)
        if self:GetVar("IAMbuilt") == true then
            self:StartPathing()
        end
    end
end

function onRebuildNotifyState(self, msg)
    if (msg.iState == 2) then
        self:SetVar("IAMbuilt", true)
        if self:GetVar("ModuleUp") == true then
            self:StartPathing()
        end
      --[[local object = self:GetObjectsInGroup{group = "PlatformSwitch1", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "PlatformBuilt", ObjIDSender = self}
      end
      local object2 = self:GetObjectsInGroup{group = "PlatformSwitch2", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:NotifyObject{name = "PlatformBuilt", ObjIDSender = self}
      end--]]
   end
end