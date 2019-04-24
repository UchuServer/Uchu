-----------------------------------------------------------------
--once the qb is built it notifies the switches that they will function on the platforms
-----------------------------------------------------------------

function onRebuildNotifyState(self, msg)

   if msg.iState == 2 then
      
      local group = self:GetObjectsInGroup{group = "Pit1Switches", ignoreSpawners = true}.objects
         
      for i, object in pairs(group) do
      
         object:NotifyObject{ name = "IAmBuilt" }
      
      end
   end
end