-----------------------------------------------------------------------
--script letting the teleporter pad know when the rebuild is complete
-----------------------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      local object = self:GetObjectsInGroup{group = "Teleporter", ignoreSpawners = true}.objects[1]
      if object then
         object:NotifyObject{name = "TeleportBuilt", ObjIDSender = self}
      end
   end
end