function onStartup(self)
   self:AddObjectToGroup{group = "spiderTrap"}
   self:SetVar("trapSet", false)
   --print("Trap B ready!")
end

function onNotifyObject(self, msg)
   if (msg.name == "trapAIsSet") then
      self:SetVar("trapSet", true)
      --print("got message from trap A!")
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
      --print("getting ready to send message to object!")
      local friends = self:GetObjectsInGroup{group = "spiderTrap", ignoreSpawners = true}.objects
      for i, object in pairs(friends) do
         if (object:GetLOT().objtemplate == 6516) then
            --print("sending message to object!")
            object:NotifyObject{name = "trapBIsSet", ObjectIDSender = self}
         end
      end
   end
end