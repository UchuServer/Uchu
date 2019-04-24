---------------------------------------------------------------------
--plays an effect when the player launches off the air bouncer
--created SY 10-07-10
---------------------------------------------------------------------

function onCollisionPhantom(self, msg)
   
   local player = msg.objectID
   
   if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
      player:PlayFXEffect{name = "leaves", effectType = "create", effectID = 4484}
   end
end