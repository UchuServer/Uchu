--------------------------------------------------------------
--script telling the switch to start the moving platform/fuse
--------------------------------------------------------------

function onStartup(self)
   --print("survival switch starting up")
   self:SetVar("BarrelUp", false)
   self:SetVar("HavePlayedOnce", false)
end

function onNotifyObject(self, msg)
   --print("switch got a message")
   if msg.name == "BarrelBuilt" then
      self:SetVar("BarrelUp", true)
   end
end

function onFireEvent(self, msg)
   --print("event fired")
   if self:GetVar("BarrelUp") == true and self:GetVar("HavePlayedOnce") == false then
        self:SetVar("HavePlayedOnce", true)
      local object = self:GetObjectsInGroup{group = "Fuse", ignoreSpawners = true}.objects[1]
      if object then
         object:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
         object:PlayFXEffect{name = "firebrick", effectID = 270, effectType = "create"}
      end
      --[[local object2 = self:GetObjectsInGroup{group = "SurRock", ignoreSpawners = true}.objects[1]
      if object2 then
         object2:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
      end--]]
      local spawnerObj = LEVEL:GetSpawnerByName("SurSpiders")
      if spawnerObj then
         spawnerObj:SpawnerActivate()
         spawnerObj:SpawnerReset()
      end
   end
end