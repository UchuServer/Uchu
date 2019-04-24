-----------------------------------------------------------------------
--script letting the survival switch know when the rebuild is complete
-----------------------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      local object = self:GetObjectsInGroup{group = "SurSwitch", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "BarrelBuilt", ObjIDSender = self}
      end
   end
end