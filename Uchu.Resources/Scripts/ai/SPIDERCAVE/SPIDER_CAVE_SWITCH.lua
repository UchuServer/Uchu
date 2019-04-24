------------------------------------------------------------
--script telling the switch to only activate once the module is complete
------------------------------------------------------------

function onStartup(self)
   self:SetVar("ModuleUp", false)
end

function onNotifyObject(self, msg)
   --print(" switch notified")
   if msg.name == "ModuleBuilt" then
      self:SetVar("ModuleUp", true)
   end
end

function onFireEvent(self, msg)
   --print("event fired")
   if --[[msg.name == "switchDown" and--]] self:GetVar("ModuleUp") == true then
      print("switch down")
      local object = self:GetObjectsInGroup{group = "platform", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
      end
   --[[elseif msg.name == "switchUp" then
      local object = self:GetObjectsInGroup{group = "platform2", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
      end--]]
   end
end