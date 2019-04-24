-----------------------------------------------------------
--script telling the effects to play on the first "cable" in the spider cave
-----------------------------------------------------------

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      local object = self:GetObjectsInGroup{group = "ModuleIntro", ignoreSpawners = true}.objects[1]
      if object then
         object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
      end
      local object2 = self:GetObjectsInGroup{group = "SwitchIntro", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
         object2:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
      end
   end
end