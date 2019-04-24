-------------------------------------------------------------------
--script to have the switches all play nice with each other when multiple people are stepping on them (they activate the same platforms)
-------------------------------------------------------------------

function onStartup(self)
   self:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
   --self:SetVar("IAMengaged", false)
   self:SetVar("Switch2IsEngaged", false)
   self:SetVar("Switch4IsEngaged", false)
end

function onNotifyObject(self, msg)
   if msg.name == "Switch2Pressed" then
      self:SetVar("Switch2IsEngaged", true)
   elseif msg.name == "Switch2Depressed" then
      self:SetVar("Switch2IsEngaged", false)
   elseif msg.name == "Switch4Pressed" then
      self:SetVar("Switch4IsEngaged", true)
   elseif msg.name == "Switch4Depressed" then
      self:SetVar("Switch4IsEngaged", false)
   end
end

function onFireEvent(self, msg)
   if msg.args == "down" then
      --self:SetVar("IAMengaged", true)
      local object = self:GetObjectsInGroup{group = "SpiderPitSwitch2", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "Switch3Pressed", ObjIDSender = self}
      end
      local object = self:GetObjectsInGroup{group = "SpiderPitSwitch4", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "Switch3Pressed", ObjIDSender = self}
      end
      local object = self:GetObjectsInGroup{group = "PitPlatform2", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
      end
      local object = self:GetObjectsInGroup{group = "PitPlatform3", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
      end
   elseif msg.args == "up" then
      --self:SetVar("IAMengaged", false)
      local object = self:GetObjectsInGroup{group = "SpiderPitSwitch2", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "Switch3Depressed", ObjIDSender = self}
      end
      local object = self:GetObjectsInGroup{group = "SpiderPitSwitch4", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "Switch3Depressed", ObjIDSender = self}
      end
      if self:GetVar("Switch2IsEngaged") == false then
         local object = self:GetObjectsInGroup{group = "PitPlatform2", ignoreSpawners = true}.objects[1]
         if object then
            object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
         end
      end
      if self:GetVar("Switch4IsEngaged") == false then
         local object = self:GetObjectsInGroup{group = "PitPlatform3", ignoreSpawners = true}.objects[1]
         if object then
            object:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
         end
      end
   end
end