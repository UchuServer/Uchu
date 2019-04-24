--------------------------------------------------------------------
--script that tells the door to open once the switch is pressed and the module is built
--------------------------------------------------------------------

function onStartup(self)
   self:SetVar("ModuleUp", false)
end

function onNotifyObject(self, msg)
   if msg.name == "ModuleBuilt" then
      --print("module built")
      self:SetVar("ModuleUp", true)
   end
end

function onFireEvent(self, msg)
   if self:GetVar("ModuleUp") == true then
      local object = self:GetObjectsInGroup{group = "SentinelGate", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 2, bAllowPathingDirectionChange = false}
      end
   end
end