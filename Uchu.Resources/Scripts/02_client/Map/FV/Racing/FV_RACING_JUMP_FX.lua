---------------------------------------------------------------------
--plays an effect when the player launches off the air bouncer
--updated SY 10-07-10
---------------------------------------------------------------------

function onCollisionPhantom(self, msg)
   
   local player = msg.objectID
   
   if player:GetID() == GAMEOBJ:GetControlledID():GetID() then
      player:PlayFXEffect{name = "wind", effectType = "create", effectID = 4449}
   end
end