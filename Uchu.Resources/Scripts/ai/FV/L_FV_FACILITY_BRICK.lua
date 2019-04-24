---------------------------------------------------------------------
--plays effects and spawns in the imagination brick in the paradox facility in FV
---------------------------------------------------------------------



function onStartup(self)

   self:SetVar("ConsoleLEFTActive", false)
   self:SetVar("ConsoleRIGHTtActive", false)

end





function onFireEventServerSide(self, msg)

   --------------------------------------------------------------
   --plays effects once it recieves a specific server side message
   --------------------------------------------------------------
   
   if msg.args ~= "PlayFX" then return end
   
   self:PlayFXEffect{name = "LeftPipeOff", effectID = 2774, effectType = "create"}
   self:PlayFXEffect{name = "RightPipeOff", effectID = 2777, effectType = "create"}
   self:PlayFXEffect{name = "imagination_canister", effectID = 2750, effectType = "create"}
   self:PlayFXEffect{name = "canister_light_filler", effectID = 2751, effectType = "create"}
end






function onNotifyObject(self, msg)

   
   local BrickSpawner = LEVEL:GetSpawnerByName("ImaginationBrick")
   local BugSpawner = LEVEL:GetSpawnerByName("MaelstromBug")
   local CanisterSpawner = LEVEL:GetSpawnerByName("BrickCanister")
   
   
   --------------------------------------------------------------
   --checking for the status of the consoles to see whether they are built and/or active
   --------------------------------------------------------------
   
   if msg.name == "ConsoleLeftUp" then
      
      self:StopFXEffect{name = "LeftPipeOff"}
      self:PlayFXEffect{name = "LeftPipeEnergy", effectID = 2775, effectType = "create"}
      
   elseif msg.name == "ConsoleLeftDown" then
      
      self:SetVar("ConsoleLEFTActive", false)
      
      self:StopFXEffect{name = "LeftPipeEnergy"}
      self:StopFXEffect{name = "LeftPipeOn"}
      self:PlayFXEffect{name = "LeftPipeOff", effectID = 2774, effectType = "create"}
   
   elseif msg.name == "ConsoleLeftActive" then
        
        self:SetVar("ConsoleLEFTActive", true)
        
        self:StopFXEffect{name = "LeftPipeEnergy"}
        self:PlayFXEffect{name = "LeftPipeOn", effectID = 2776, effectType = "create"}
   
   elseif msg.name == "ConsoleRightUp" then
      
      self:StopFXEffect{name = "RightPipeOff"}
      self:PlayFXEffect{name = "RightPipeEnergy", effectID = 2778, effectType = "create"}
      
   elseif msg.name == "ConsoleRightDown" then
      
      self:SetVar("ConsoleRIGHTActive", false)
      
      self:StopFXEffect{name = "RightPipeEnergy"}
      self:StopFXEffect{name = "RightPipeOn"}
      self:PlayFXEffect{name = "RightPipeOff", effectID = 2777, effectType = "create"}
   
   elseif msg.name == "ConsoleRightActive" then
        
        self:SetVar("ConsoleRIGHTActive", true)
        
        self:StopFXEffect{name = "RightPipeEnergy"}
        self:PlayFXEffect{name = "RightPipeOn", effectID = 2779, effectType = "create"}
   
   end
   
   
   
   --------------------------------------------------------------
   --playing effects and/or spawning/killing objects based on whether or not the consoles are active
   --------------------------------------------------------------
   
   if self:GetVar("ConsoleLEFTActive") == true and self:GetVar("ConsoleRIGHTActive") == true then
      
      --------------------------------------------------------------
      --play effects on the blue brick and destroy the canister that
      --it is inside of if both consoles are active
      --------------------------------------------------------------
      
      local object = self:GetObjectsInGroup{group = "Brick", ignoreSpawners = true}.objects[1]
      
      if object then
         
         object:PlayFXEffect{name = "bluebrick", effectID = 122, effectType = "create"}
         object:PlayFXEffect{name = "imaginationexplosion", effectID = 1034, effectType = "cast"}
         
      end
      
      local object = self:GetObjectsInGroup{group = "Canister", ignoreSpawners = true}.objects[1]
      
      if object then
         
         object:RequestDie{killType = "SILENT"}
         
      end
      
      CanisterSpawner:SpawnerDestroyObjects()
      CanisterSpawner:SpawnerDeactivate()
      
   elseif self:GetVar("ConsoleLEFTActive") == true or self:GetVar("ConsoleRIGHTActive") == true then
      
      --------------------------------------------------------------
      --spawn the brick in and kill off the fish if only one console is active
      --also bring the canister back in if it was removed while the brick was in
      --------------------------------------------------------------
      
      BrickSpawner:SpawnerActivate()
      
      local object = self:GetObjectsInGroup{group = "Brick", ignoreSpawners = true}.objects[1]
      
      if object then
         
         object:StopFXEffect{name = "bluebrick"}
         
      end
      
      BugSpawner:SpawnerDestroyObjects()
      BugSpawner:SpawnerDeactivate()
      
      CanisterSpawner:SpawnerReset()
      CanisterSpawner:SpawnerActivate()
      
   else
      
      BrickSpawner:SpawnerDestroyObjects()
      BrickSpawner:SpawnerDeactivate()
      BrickSpawner:SpawnerReset()
      
      BugSpawner:SpawnerReset()
      BugSpawner:SpawnerActivate()
      
   end

end