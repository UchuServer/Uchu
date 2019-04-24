-----------------------------------------------------------
-- client-side script for the FV race track bouncer
-- Steve Y .. 11-10-10
-----------------------------------------------------------

function onCollisionPhantom(self, msg)
   
   local player = msg.objectID
   
   if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
      player:PlayFXEffect{name = "camShake", effectType = "cast", effectID = 4765}
      player:StopFXEffect{name = "camShake"}
   end
end