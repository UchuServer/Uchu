-----------------------------------------------------------
--script telling the effects to play on the first "cable" in the spider cave
-----------------------------------------------------------

function onRebuildNotifyState(self, msg)
    if msg.iState == 2 then
        self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
      --[[local group = self:GetObjectsInGroup{group = "Switch3", ignoreSpawners = true}.objects
      for i, object2 in pairs(group) do
         if object2 then
            object2:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
            object2:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
         end
      end--]]
        local group = self:GetObjectsInGroup{group = "Switch4", ignoreSpawners = true}.objects
        for i, object3 in pairs(group) do
            if object3 then
                object3:NotifyObject{name = "ModuleBuilt", ObjIDSender = self}
                object3:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end
        end
        local object4 = self:GetObjectsInGroup{group = "SpiderPlatform", ignoreSpawners = true}.objects[1]
        if object4 then  
            print("found object")
            object4:StartPathing()
        end
    end
end