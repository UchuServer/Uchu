---------------------------------------------------------------
--script for the turret telling the switches when it is built
---------------------------------------------------------------

function onRebuildNotifyState(self, msg)

   if msg.iState == 2 then
      
      local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
      
      for i, object in pairs(group) do
         
         if object then
            object:NotifyObject{name = "TurretBuilt", ObjIDSender = self}
         end
      end
   end
end