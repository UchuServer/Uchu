-----------------------------------------------------------------------
--script to change the camera when the player enters a volume in the first spider encounter area
-----------------------------------------------------------------------

function onCollisionPhantom(self, msg)
   local playerID = GAMEOBJ:GetLocalCharID()
   if (msg.objectID:GetID() == playerID) then
      --local playerAsID = GAMEOBJ:GetLocalCharID()
      --local player = GAMEOBJ:GetObjectByID(playerAsID)
      CAMERA:ActivateCamera("CAMERA_SIDE_SCROLLER")
      CAMERA:SetRenderCamera("CAMERA_SIDE_SCROLLER")
	  CAMERA:SetCameraLookAtPoint("CAMERA_SIDE_SCROLLER", -1, -0.25, -0.3)
	  CAMERA:SetCameraZoom("CAMERA_SIDE_SCROLLER", 90)
   end
end