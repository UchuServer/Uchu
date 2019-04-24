function onStartup(self)
   self:AddObjectToGroup{group = "spiderTrap"}
   self:SetVar("trapSet", false)
   --print("Trap A ready!")
end

function onNotifyObject(self, msg)
   if (msg.name == "trapBIsSet") then
      self:SetVar("trapSet", true)
      --print("got message from trap B!")
   end
end

function onRebuildNotifyState(self, msg)
   if (msg.iState == 2) and (self:GetVar("trapSet") == true) then
      local friends = self:GetObjectsInGroup{group = "spiderTrap", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if (object:GetLOT().objtemplate == 6457) then
            object:Die{killerID = self, killType = "VIOLENT"}
            --print("Spider Dead!")
         elseif  (object:GetLOT().objtemplate == 5651) then
            object:ToggleTrigger{enable = false}
         end
      end
   elseif (msg.iState == 2) then
      local friends = self:GetObjectsInGroup{group = "spiderTrap", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if (object:GetLOT().objtemplate == 6517) then
            object:NotifyObject{name = "trapAIsSet"}
         end
      end
   end
end