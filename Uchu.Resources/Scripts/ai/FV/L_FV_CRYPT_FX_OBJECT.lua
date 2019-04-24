--------------------------------------------------------------------------
--object playing effects in the crypt scene
--------------------------------------------------------------------------



function onFireEventServerSide(self, msg)

   if msg.args ~= "DoorReady" then return end
   
   self:PlayFXEffect{name = "smoke", effectID = 2847, effectType = "create"}
end





function onNotifyObject(self, msg)

   --print("got message")
   
   if msg.name == "DoorUp" then
      
      --print("door is up")
      
      self:PlayFXEffect{name = "door", effectID = 2844, effectType = "create"}
      self:StopFXEffect{name = "smoke"}
   
   elseif msg.name == "DoorDown" then
      
      --print("door is down")
      
      self:PlayFXEffect{name = "smoke", effectID = 2847, effectType = "create"}
      self:StopFXEffect{name = "door"}
      
   end

end