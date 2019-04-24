--------------------------------------------------------------------
--script for switch telling the horizontal moving platforms to stop pathing
--------------------------------------------------------------------

function onStartup(self)

   self:SetVar("SentinelModuleBuilt", false)

end



function onNotifyObject(self, msg)

   if msg.name == "IAmBuilt" then
      
      self:SetVar("SentinelModuleBuilt", true)
      self:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
   
   end
end



function onFireEvent(self, msg)

   if self:GetVar("SentinelModuleBuilt") == true then
      
      if msg.args == "down" then
         
         local group = self:GetObjectsInGroup{group = "Pit1Platforms", ignoreSpawners = true}.objects
         
         for i, object in pairs(group) do
         
            object:StopPathing()
         
         end
      
      elseif msg.args == "up" then
      
         local group = self:GetObjectsInGroup{group = "Pit1Platforms", ignoreSpawners = true}.objects
         
         for i, object in pairs(group) do
         
            object:StartPathing()
         
         end
      end
   end
end