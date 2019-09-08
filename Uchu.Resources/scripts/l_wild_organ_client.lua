require('o_mis')

function onStartup(self)

    -- Resetting picktype
    self:SetVar("organuseclient", 0)
    self:RequestPickTypeUpdate()
    -- REMOVED FOR NOW - Make sure organ is playing its idle animation
    -- REMOVED FOR NOW - self:PlayAnimation{ animationID = "key-up" }

end

function onGetPriorityPickListType(self, msg)

    if self:GetVar("organuseclient") == 1 then
        msg.ePickType = -1 -- Non-Interactive pick type
        return msg
    else
        msg.ePickType = 14 -- Interactive pick type
        return msg
    end

end

function onNotifyClientObject(self, msg)      

    if msg.name == "organstart" then
        -- Make the client unclickable for everyone else that tries to use it
        self:SetVar("organuseclient", 1)
        self:RequestPickTypeUpdate()
    end

end

function UpdateBeingPlayed( self )

	local player = getObjectByName(self,"OrganUserOnClient")
	local Imagination = player:GetImagination{}.imagination

	-- Check whether the player as run out of imagination
	if Imagination > 0 then 
        bMovement = DidPlayerHitMovementKey(self)
        if bMovement == true then
            -- If the player hit a movement key, then send a server side message to kick them off the organ
            self:FireEventServerSide{senderID = self, args = "PlayerMoved"}
        else
            -- If the player hasn't moved, set a new timer to check again
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "CheckOnPlayer", self )
        end	
    end

end

function TrackRecentMovementKeys( self, bTrack )	
    -- Actual tracking of key presses
	local player = getObjectByName(self,"OrganUserOnClient")
    player:TrackRecentMovementKeys{ bTrackForward = bTrack, bTrackReverse = bTrack, bTrackLeft= bTrack, bTrackRight = bTrack, bTrackJump = bTrack }	

end

function DidPlayerHitMovementKey( self )
    -- Return some value if key was pressed
	local player = getObjectByName(self,"OrganUserOnClient")
	local keysMsg = player:GetRecentMovementKeys{}

	return ( keysMsg.bForwardPressed or keysMsg.bReversePressed or keysMsg.bLeftPressed or keysMsg.bRightPressed or keysMsg.bJumpPressed )

end

function onTimerDone(self, msg)

    if ( msg.name == "CheckOnPlayer" ) then
        -- Keep checking to see if player moved
        UpdateBeingPlayed( self )
    end

end

function onEnableActivity(self, msg)

    if (msg.bEnable == true) then

        local player = GAMEOBJ:GetControlledID()
   
       --Relative position/rotation data
        local oPos = { pos = "", rot = ""}
        local oDir = self:GetObjectDirectionVectors()
        oPos.pos = self:GetPosition().pos
        oPos.rot = self:GetRotation()

        -- Storing vectors for player
        oPos.pos.x = oPos.pos.x + (oDir.forward.x * 2.107)
        oPos.pos.z = oPos.pos.z + (oDir.forward.z * 2.107)

        print "Activity Enabled"

        -- Toggle variable for picktype
        self:SetVar("organuseclient", 1)

        -- Store the player name
        storeObjectByName(self, "OrganUserOnClient", player)

        -- Stun the Player since camera changes forbid interactions
        player:SetStunned{StateChangeType = "PUSH", bCantMove = true, bCantAttack = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}

        -- Make player transparent
        getObjectByName(self,"OrganUserOnClient"):StopFXEffect{name = "untransparent"}
        player:PlayFXEffect{name = "transparent", effectID = 2754, effectType = "trans"}

        -- REMOVED FOR NOW - Make the organ drop obstructing geometry for the camera angle and server-side fake keys
        -- REMOVED FOR NOW - self:PlayAnimation{animationID = "key-down"}

        -- REMOVED FOR NOW - Play the organ camera angle for awesomeness
        -- REMOVED FOR NOW - player:PlayCinematic{pathName = "OrganCam"}

        -- Place the player on the organ bench
        player:SetPosition{pos = {x = oPos.pos.x, y = oPos.pos.y, z = oPos.pos.z}}
        player:SetRotation{x=oPos.rot.x, y=oPos.rot.y, z=oPos.rot.z, w=oPos.rot.w}

        -- Start the organ animation idle set
        player:ChangeIdleFlags{on = 13}

        -- Turn off the icon above the organ during play
        self:SetIconAboveHead{bIconOff = true, iconMode = 1, iconType = 69}

        -- Change the picktype to make 
        self:RequestPickTypeUpdate()

        -- Flip the player 180
        local newRot = self:GetPosition{}.pos
        newRot.x = 0.0
        newRot.y = 3.1416
        newRot.z = 0.0
        player:RotateObject{rotation = newRot}

        -- Start the move-check timer and key tracking
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "CheckOnPlayer", self )
        TrackRecentMovementKeys(self, true)	

    else
        print "Disabling activity"

        -- REMOVED FOR NOW - Restore the organ visuals on client
        -- REMOVED FOR NOW - self:PlayAnimation{ animationID = "key-up" }

        -- Turn the icon back on
        self:SetIconAboveHead{iconMode = 1, iconType = 69, bIconOff = false}

        -- Unstun player
        getObjectByName(self,"OrganUserOnClient"):SetStunned{StateChangeType = "POP", bCantMove = true, bCantAttack = true, bIgnoreImmunity = true, bCantTurn = true, bCantEquip = true}

        --Make player untransparent
        getObjectByName(self,"OrganUserOnClient"):StopFXEffect{name = "transparent"}
        getObjectByName(self,"OrganUserOnClient"):PlayFXEffect{name = "untransparent", effectID = 2754, effectType = "untrans"}

        -- REMOVED FOR NOW - Cancel camera
        -- REMOVED FOR NOW - getObjectByName(self,"OrganUserOnClient"):EndCinematic{ leadOut = 1.0 }

        --Restore animation set to normal
        getObjectByName(self,"OrganUserOnClient"):ChangeIdleFlags{off = 13}

        -- Stop tracking keypresses
        TrackRecentMovementKeys( self, false )

        -- Purge player from memory
        self:SetVar("OrganUserOnClient", nil)

        -- Make organ clickable again
        self:SetVar("organuseclient", 0)
        self:RequestPickTypeUpdate()
    end 

end
