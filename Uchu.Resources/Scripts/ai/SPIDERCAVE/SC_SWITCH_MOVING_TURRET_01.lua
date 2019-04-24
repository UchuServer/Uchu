-----------------------------------------------------------
-- Script on switch which and moves to platform waypoint
-- Updated 3/15 Darren McKinsey
-----------------------------------------------------------
function onStartup(self)
   self:SetVar("ModuleUp", false)
end

function onNotifyObject(self, msg)
   if msg.name == "ModuleBuilt" then
      self:SetVar("ModuleUp", true)
   end
end

function onFireEvent(self, msg)
   if self:GetVar("ModuleUp") == true then
      local object = self:GetObjectsInGroup{group = "moving_turret_01", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
      end
   end
end