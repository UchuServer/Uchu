-----------------------------------------------------------
--script telling the effects to play on the first "cable" in the spider cave
-----------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      local group = self:GetObjectsInGroup{group = "Module4", ignoreSpawners = true}.objects
      for i, object in pairs(group) do
         if object then
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
         end
      end
      local object3 = self:GetObjectsInGroup{group = "SpiderBoss1", ignoreSpawners = true}.objects[1]
      if object3 then
         object3:NotifyObject{name = "shocked", ObjIDSender = self}
      end
      local group2 = self:GetObjectsInGroup{group = "Switch5", ignoreSpawners = true}.objects
      for i, object2 in pairs(group2) do
         if object2 then
            object2:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
            object2:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
         end
      end
   end
end