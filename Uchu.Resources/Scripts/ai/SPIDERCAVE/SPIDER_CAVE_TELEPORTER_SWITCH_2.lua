-------------------------------------------------------------
--script telling the other switch to not send the teleport back when depressed
-------------------------------------------------------------

function onStartup(self)
   self:SetVar("IAMengaged", false)
   self:SetVar("HEISengaged", false)
end

function onNotifyObject(self, msg)
   if msg.name == "pressed" then
      self:SetVar("HEISengaged", true)
   elseif msg.name == "depressed" then
      self:SetVar("HEISengaged", false)
   end
end

function onFireEvent(self, msg)
   if msg.args == "down" then
      self:SetVar("IAMengaged", true)
      local object = self:GetObjectsInGroup{group = "teleportSwitch1", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "pressed", ObjIDSender = self}
      end
      local object = self:GetObjectsInGroup{group = "teleport", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
      end
   elseif msg.args == "up" then
      self:SetVar("IAMengaged", false)
      local object2 = self:GetObjectsInGroup{group = "teleportSwitch1", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:NotifyObject{name = "depressed", ObjIDSender = self}
      end
      if self:GetVar("HEISengaged") == false then
         local object = self:GetObjectsInGroup{group = "teleport", ignoreSpawners = true}.objects[1]
         if object then
            object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
         end
      end
   end
end