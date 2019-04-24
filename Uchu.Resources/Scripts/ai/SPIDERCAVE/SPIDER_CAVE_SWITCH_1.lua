------------------------------------------------------------
--script telling the switch to only activate once the module is complete
------------------------------------------------------------

function onStartup(self)
    self:SetVar("ModuleUp", false)
    self:SetVar("PlatformUp", false)
    self:SetVar("IAMengaged", false)
    self:SetVar("HEISengaged", false)
end

function onNotifyObject(self, msg)
   --print(" switch notified")
   if msg.name == "ModuleBuilt" then
      self:SetVar("ModuleUp", true)
   elseif msg.name == "PlatformBuilt" then
      self:SetVar("PlatformUp", true)
    elseif msg.name == "pressed" then
      self:SetVar("HEISengaged", true)
   elseif msg.name == "depressed" then
      self:SetVar("HEISengaged", false)
   end
end

function onFireEvent(self, msg)
    print("switch activated")
   if msg.args == "down" and self:GetVar("ModuleUp") == true and self:GetVar("PlatformUp") == true then
    self:SetVar("IAMengaged", true)
      local object = self:GetObjectsInGroup{group = "PlatformSwitch2", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "pressed", ObjIDSender = self}
      end
      local object = self:GetObjectsInGroup{group = "platform2", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
      end
   elseif msg.args == "up" then
      self:SetVar("IAMengaged", false)
      local object2 = self:GetObjectsInGroup{group = "PlatformSwitch2", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:NotifyObject{name = "depressed", ObjIDSender = self}
      end
      if self:GetVar("HEISengaged") == false then
         local object = self:GetObjectsInGroup{group = "platform2", ignoreSpawners = true}.objects[1]
         if object then
            object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
         end
      end
   end
end