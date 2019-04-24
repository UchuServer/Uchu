-----------------------------------------------------------------------
--script ending the side scrolling camera
-----------------------------------------------------------------------

function onCollisionPhantom(self, msg)
   local playerID = GAMEOBJ:GetLocalCharID()
   if (msg.objectID:GetID() == playerID) then
      CAMERA:SetToPrevGameCam()
   end
end