--------------------------------------------------------------------
--script to turn the effects on and off for the gate
--------------------------------------------------------------------


function onStartup(self)

   self:AddObjectToGroup{group = "Gate"}
   self:SetVar("AmPlayingFX", false)

end



function onNotifyObject(self, msg)

   if msg.name == "GateActive" and self:GetVar("AmPlayingFX") == false then
      
      --print("playing FX")
      
      self:SetVar("AmPlayingFX", true)
      
      self:PlayFXEffect{effectID = 2988, name = "GateSpawn", effectType = "create"}
      
      GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "StartFX", self )
      
   elseif msg.name == "GateDown" and self:GetVar("AmPlayingFX") then
      
      --print("shutting off FX")
      
      self:SetVar("AmPlayingFX", false)
      
      self:StopFXEffect{name = "GateEnergy"}
      self:StopFXEffect{name = "GateEnergy2"}
      --self:StopFXEffect{name = "GateSpawn"}

   end

end



function onTimerDone(self, msg)

   if msg.name == "StartFX" then
      
      self:PlayFXEffect{effectID = 2242, name = "GateEnergy2", effectType = "create"}
      self:PlayFXEffect{effectID = 2243, name = "GateEnergy", effectType = "create"}
   
   end

end