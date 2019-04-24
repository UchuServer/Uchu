--------------------------------------------------------------

-- L_CAM_RAIL_CLIENT.lua

-- Client side script for camera volume processing while
-- travelling along a rail.
-- created abeechler ... 8/23/11

--------------------------------------------------------------

local defaultAttachLocked = false       -- Allows the script to prevent offCollision messages while the player 
                                        -- is travelling on a rail

----------------------------------------------------------------
-- Catch off collision messages and process to determine whether
-- or not we should relinguish control of the camera
----------------------------------------------------------------
function onOffCollisionPhantom(self, msg)
    -- Acquire a reference to the local player
    local player = GAMEOBJ:GetControlledID()
    local playerID = player:GetID()
    
    -- Is the sender the local client?
    if(msg.senderID:GetID() == playerID) then
        local bAttachLocked = self:GetVar("bAttachLocked") or defaultAttachLocked
        
        -- Are we locked cuurently by rail travel?
        if(not bAttachLocked) then
            local cameraPath = self:GetVar("cameraPath") or false
            local leadOut = self:GetVar("leadOut") or -1.0
            
            if(cameraPath) then
                -- We have valid config data - remove volume camera control
                player:DetachCameraFromRail{pathName = cameraPath, leadOut = leadOut}
            end
            
            -- Remove player script event notifications
            self:SendLuaNotificationCancel{requestTarget = player, messageName = "StartRailMovement"}
	        self:SendLuaNotificationCancel{requestTarget = player, messageName = "CancelRailMovement"}
	        -- Allow subsequent notification requests
	        self:SetVar("subscribeLock", false)
        end
    end
end

----------------------------------------------------------------
-- Catch on collision messages, assuming control of the camera 
-- when we have valid information to process
----------------------------------------------------------------
function onCollisionPhantom(self, msg)
    -- Acquire a reference to the local player
    local player = GAMEOBJ:GetControlledID()
    local playerID = player:GetID()
    
    -- Is the sender the local client?
    if(msg.senderID:GetID() == playerID) then
            local cameraPath = self:GetVar("cameraPath") or false
            local positionPath = self:GetVar("positionPath") or false
            local alwaysFaceTarget = self:GetVar("alwaysFaceTarget") or false
            local leadIn = self:GetVar("leadIn") or -1.0
            local biasAmount = self:GetVar("biasAmount") or 0
        
            if(cameraPath and positionPath) then
                -- We have valid config data - assume volume camera control
                player:AttachCameraToRail{pathName = cameraPath, positionPathName = positionPath, leadIn = leadIn, alwaysFaceTarget = alwaysFaceTarget, targetID = player, biasAmount = biasAmount}
            end
            
            -- Obtain the subscribe lock to determine whether or not we have a current 
            -- notification subscription
            local subscribeLock = self:GetVar("subscribeLock") or false
            if(not subscribeLock) then
                -- Subscribe to player script event notifications
	            self:SendLuaNotificationRequest{requestTarget = player, messageName = "StartRailMovement"}
	            self:SendLuaNotificationRequest{requestTarget = player, messageName = "CancelRailMovement"}
	            -- Prevent subsequent notification requests
	            self:SetVar("subscribeLock", true)
	        end
    end
end

----------------------------------------------------------------
-- Determine when the local player is travelling along a rail
-- to prevent off collision events
----------------------------------------------------------------
function notifyStartRailMovement(self, player, msg)
    self:SetVar("bAttachLocked", true)
end

----------------------------------------------------------------
-- Determine when the local player is done travelling along a rail
-- to allow off collision events
----------------------------------------------------------------
function notifyCancelRailMovement(self, player, msg)
    self:SetVar("bAttachLocked", false)
end
