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
        CAMERA:SetCameraLookAtPoint("CAMERA_SIDE_SCROLLER", 0, -0.3, 1)
        CAMERA:SetCameraZoom("CAMERA_SIDE_SCROLLER", 50)
        
        UI:SendMessage("GameStateChanged", {{"user",tostring(playerID) }} )
        UI:SendMessage("ToggleActivityCloseButton", {{"Toggle",true}})
    end
end