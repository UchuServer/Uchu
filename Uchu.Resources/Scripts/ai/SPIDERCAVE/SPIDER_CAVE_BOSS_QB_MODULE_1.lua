------------------------------------------------------------------------
--script for the spider cave QB boss module 1
------------------------------------------------------------------------



function onStartup(self)
   self:SetVar("IAmBuilt", false)
   self:SetVar("SpiderSpawned", false)
   self:SetVar("CoilSmashDown", false)
   self:SetVar("Mod2SmashDown", false)
   self:SetVar("ModuleTwoIsBuilt", true)
   self:SetVar("CoilModIsBuilt", true)
   --print("qb module 1 starting up!")
end



function onRebuildNotifyState(self, msg)

   --print("updating rebuild state!")
   
   if msg.iState == 2 then
      self:SetVar("IAmBuilt", true)
      local object = self:GetObjectsInGroup{group = "FinalModule", ignoreSpawners = true}.objects[1]
      
      if object then
         object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
      end
      
      if self:GetVar("SpiderSpawned") == false then
         --print("spawning the spider!")
         self:SetVar("SpiderSpawned", true)
         local spawnerObj = LEVEL:GetSpawnerByName("SpiderBoss2")
        
         if spawnerObj then
            --print("spawning spider boss!")
            spawnerObj:SpawnerActivate()
            --spawnerObj:SpawnerDeactivate()
         end
      end
      
      if self:GetVar("ModuleTwoIsBuilt") == true and self:GetVar("CoilModIsBuilt") == true then
         --------------------------------------------------------------------------------
         --telling the switches to enable their effects and that they are ready to fire--
         --------------------------------------------------------------------------------
         
         print("everything is up!")
         
         local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
         
         for i, object in pairs(group) do
         
            if object then
               object:NotifyObject{name = "AllUp", ObjIDSender = self}
               object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            end
         end
         
         if self:GetVar("Mod2SmashDown") == false then
            -----------------------------------------------------------------
            --tell the 2nd smashable module that it is ready for the effect--
            -----------------------------------------------------------------
            
            local object = self:GetObjectsInGroup{group = "FinalModSmash2", ignoreSpawners = true}.objects[1]
            
            if object then
               object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            end
         
         elseif self:GetVar("Mod2SmashDown") == true then
            -----------------------------------------------------------------
            --tell the 2nd module QB that it is ready for the effect--
            -----------------------------------------------------------------
         
            local object = self:GetObjectsInGroup{group = "FinalModule2", ignoreSpawners = true}.objects[1]
            
            if object then
               object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
            end
         end
      end
   
   elseif msg.iState == 4 then
      ------------------------------------------------------------------
      --tell all modules and switches that the main power is destroyed--
      ------------------------------------------------------------------
      
      print("module 1 destroyed!")
      self:SetVar("IAmBuilt", false)
      --------------------------------------------------------------------------------
      --telling the switches to enable their effects and that they are ready to fire--
      --------------------------------------------------------------------------------
      
      local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
      
      for i, object in pairs(group) do
      
         if object then
            object:NotifyObject{name = "AllDown", ObjIDSender = self}
            object:StopFXEffect{name = "sirenlight_B"}
         end
      end
      
      -----------------------------------------------------------------
      --tell the module that it is stopping the effect--
      -----------------------------------------------------------------
      
      local object = self:GetObjectsInGroup{group = "FinalModule", ignoreSpawners = true}.objects[1]
      
      if object then
         object:StopFXEffect{name = "moviespotlight"}
      end
      
      if self:GetVar("Mod2SmashDown") == false then
         -----------------------------------------------------------------
         --tell the 2nd smashable module that it is stopping the effect--
         -----------------------------------------------------------------
         
         local object = self:GetObjectsInGroup{group = "FinalModSmash2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:StopFXEffect{name = "moviespotlight"}
         end
      
      elseif self:GetVar("Mod2SmashDown") == true then
         -----------------------------------------------------------------
         --tell the 2nd module QB that it is stopping the effect if the 2nd
         --smashable hasn't been smashed yet--
         -----------------------------------------------------------------
      
         local object = self:GetObjectsInGroup{group = "FinalModule2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:StopFXEffect{name = "moviespotlight"}
         end
      end
   end
end



function onNotifyObject(self, msg)
   
   if msg.name == "ModuleTwoSmashDown" then
      self:SetVar("Mod2SmashDown", true)
      self:SetVar("ModuleTwoIsBuilt", false)
      print("module 2 smashable down")
   
   elseif msg.name == "ModuleTwoDown" then
      self:SetVar("ModuleTwoIsBuilt", false)
      print("module 2 down")
   
   elseif msg.name == "ModuleTwoUp" then
      self:SetVar("ModuleTwoIsBuilt", true)
      print("module 2 up")
      
   elseif msg.name == "ModuleCoilSmashDown" then
      self:SetVar("CoilSmashDown", true)
      self:SetVar("CoilModIsBuilt", false)
      print("coil module smashable is down")
   
   elseif msg.name == "ModuleCoilDown" then
      self:SetVar("CoilModIsBuilt", false)
      print("coil module is down")
      
   elseif msg.name == "ModuleCoilUp" then
      self:SetVar("CoilModIsBuilt", true)
      print("coil module is up")
   end
   
   if self:GetVar("IAmBuilt") == true and self:GetVar("ModuleTwoIsBuilt") == true and self:GetVar("CoilModIsBuilt") == true then
      --------------------------------------------------------------------------------
      --telling the switches to enable their effects and that they are ready to fire--
      --------------------------------------------------------------------------------
      
      print("everything is up!")
      
      local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
      
      for i, object in pairs(group) do
      
         if object then
            object:NotifyObject{name = "AllUp", ObjIDSender = self}
            object:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
         end
      end
      
      if self:GetVar("Mod2SmashDown") == false then
         -----------------------------------------------------------------
         --tell the 2nd smashable module that it is ready for the effect--
         -----------------------------------------------------------------
         
         local object = self:GetObjectsInGroup{group = "FinalModSmash2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
         end
      
      elseif self:GetVar("Mod2SmashDown") == true then
         -----------------------------------------------------------------
         --tell the 2nd module QB that it is ready for the effect--
         -----------------------------------------------------------------
      
         local object = self:GetObjectsInGroup{group = "FinalModule2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
         end
      end
   
   elseif self:GetVar("ModuleTwoIsBuilt") == false or self:GetVar("CoilModIsBuilt") == false then
      
      --------------------------------------------------------------------------------
      --telling the switches to enable their effects and that they are ready to fire--
      --------------------------------------------------------------------------------
      
      local group = self:GetObjectsInGroup{group = "FinalSwitch", ignoreSpawners = true}.objects
      
      for i, object in pairs(group) do
      
         if object then
            object:NotifyObject{name = "AllDown", ObjIDSender = self}
            object:StopFXEffect{name = "sirenlight_B"}
         end
      end
      
      if self:GetVar("Mod2SmashDown") == false then
         -----------------------------------------------------------------
         --tell the 2nd smashable module that it is stopping the effect--
         -----------------------------------------------------------------
         
         local object = self:GetObjectsInGroup{group = "FinalModSmash2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:StopFXEffect{name = "moviespotlight"}
         end
      
      elseif self:GetVar("Mod2SmashDown") == true then
         -----------------------------------------------------------------
         --tell the 2nd module QB that it is stopping the effect if the 2nd
         --smashable hasn't been smashed yet--
         -----------------------------------------------------------------
      
         local object = self:GetObjectsInGroup{group = "FinalModule2", ignoreSpawners = true}.objects[1]
         
         if object then
            object:StopFXEffect{name = "moviespotlight"}
         end
      end
   end
end