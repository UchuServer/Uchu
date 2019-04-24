

---------------------------------------------------------------------------------------------------------
-- Client-side script for Concert instrument Quick Builds.
-- When an instrument is built, it locks the mini-fig into an imagination-draining animation.
-- This lasts until the player runs out of imagination or moves voluntarily.
-- Players will hear different music depending on which instruments are currently being played.
---------------------------------------------------------------------------------------------------------




--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')





--------------------------------------------------------------
-- constants
--------------------------------------------------------------

-- copied from enum ERebuildChallengeState in lwoGame \ include \ LWORebuild.h
CONSTANTS["REBUILD_STATE_COMPLETED"] = 2		-- Challenge complete

CONSTANTS["UPDATE_FREQUENCY"] = 0.1				-- while playing the instrument, how often we check whether the player hit a movement key to stop playing

	

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )

	self:SetVar( "bBeingPlayed", false )
	self:SetVar( "activePlayer", nil )
	
	ResetEquippableIDs( self )
	
	self:SetVar( "checkingInstrumentAnim", false )
	self:SetVar( "targetPositionX", nil  )
	self:SetVar( "targetPositionZ", nil  )
	
	self:SetVar( "leftInventoryObjectID", nil )
	self:SetVar( "rightInventoryObjectID", nil )
	
	self:SetVar( "reequipPlayer", nil )
	
end




--------------------------------------------------------------
-- called anytime the rebuild object's state changes
--------------------------------------------------------------
function onRebuildNotifyState( self, msg)


	-- make sure we didn't attach this script to some object besides our quickbuild instruments
	local instrumentLOT = self:GetLOT().objtemplate
	if ( IsValidInstrument( instrumentLOT ) == false ) then
		return
	end
	
	
    
	-- When the Quick Build is completed, the player gets to play it if they have any imagination    
    if ( msg.iState == CONSTANTS["REBUILD_STATE_COMPLETED"] ) then
		storeObjectByName( self, "activePlayer", msg.player )

		QuickBuildWasBuilt( self )
	
	end

end





function QuickBuildWasBuilt( self )

	-- take control away from the player
	SetPlayerControl( self, false )

	
	-- set timers for when to hide the completed quickbuild and show the player playing the instrument
	-- do this quicky so we don't have to watch the finished quickbuild model to slam into place
	-- but we have to wait slightly first or get overridden by the quickbuild code
		
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Hide", self )
	
	
	if ( IsLocalPlayerActive( self ) == true ) then 
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Play", self )
	end

end




--------------------------------------------------------------
-- checks whether the given LOT is one of the instrument quickbuilds
--------------------------------------------------------------
function IsValidInstrument( LOT )

	return ( LOT == CONSTANTS["INSTRUMENT_LOT_GUITAR"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_BASS"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_DRUM"] )
	
end



--------------------------------------------------------------
-- show the camera panning around the concert area
--------------------------------------------------------------
function PlayCinematic( self, instrumentLOT )
   
	local player = GetActivePlayer( self )
	if ( player ~= nil ) then
		
		local szPathName = CONSTANTS["INSTRUMENT_CINEMATIC"][instrumentLOT]

		player:PlayCinematic { pathName = szPathName }
	end
	
end





--------------------------------------------------------------
-- play the animation of the mini-fig playing the given instrument
--------------------------------------------------------------
function PlayInstrumentAnim( self, instrumentLOT )
	
	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end
	
	-- start the animation
	local player = GetActivePlayer( self )
	if ( player ~= nil ) then
		self:FireEventServerSide{ senderID = player, args = "playInstrumentAnim", param1 = instrumentLOT }	
	end
	  
end




--------------------------------------------------------------
-- add or remove music according to which instrument someone just started or stopped playing
-- pass true to bActivate to turn the music on, or false to turn it off.
--------------------------------------------------------------
function AffectMusic( self, instrumentLOT, bActivate)

	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end
	
   
    local szMusicName = CONSTANTS["INSTRUMENT_MUSIC"][instrumentLOT]
      

	local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )
	if ( player ~= nil ) then
	
		local instrumentLOT = self:GetLOT().objtemplate
		
		
		if ( bActivate ) then
			self:FireEventServerSide{ senderID = player, args = "startMusic", param1 = instrumentLOT }
		
		else
			self:FireEventServerSide{ senderID = player, args = "stopMusic", param1 = instrumentLOT }
		
		end

	end
	
end




--------------------------------------------------------------
-- start the process of seeing the player playing the instrument
--------------------------------------------------------------
function PlayInstrument( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then       
		return
	end
	
	

	local instrumentLOT = self:GetLOT().objtemplate
	

	-- remember that someone is using this instrument
	self:SetVar( "bBeingPlayed", true )
	
	
	-- load any related equippables (guitar, bass, drumsticks), which will equip onto the player
	--self:FireEventServerSide{ args = "swapEquippables", senderID = player }
	RememberInventoryItems( self )
	
	
	-- start keeping track of movement key presses long enough to know if the player wants to break out of playing the instument
	TrackRecentMovementKeys( self, true )
	
		
	-- show the cinematic
	PlayCinematic( self, instrumentLOT )
	

	-- play the animation of the mini-fig playing the given instrument
	ResetPlayersAnim( self )	-- try to stop the quickbuild victory anim
	PlayInstrumentAnim( self, instrumentLOT )
    	
    
	-- start the appropriate music
	AffectMusic( self, instrumentLOT, true )


	-- start a timer for when to check whether the instrument is still being played
	StartUpdateTimer( self )
	
	
	-- start a timer for decreasing the player's imagination
	StartImaginationCostTimer( self )

end




--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone( self, msg )

	if ( msg.name == "Hide" ) then
		HideQuickbuild( self, self:GetLOT().objtemplate )
		RepositionPlayer( self, self:GetLOT().objtemplate )
			
	
	elseif ( msg.name == "Play" ) then
		PlayInstrument( self )
		
	
    elseif ( msg.name == "CheckOnPlayer" ) then
		UpdateBeingPlayed( self )
		
	
    elseif ( msg.name == "DecreaseImagination" ) then
		DecreasePlayersImagination( self )	
	
	
	elseif ( msg.name == "EndPlayer" ) then
		EndActivePlayer( self )
	
		
	elseif ( msg.name == "Unequip" ) then
		SetPlayerControl( self, true )
		UnequipEquippables( self )

	elseif( msg.name == "checkAnim" ) then
		CheckInstrumentAnim( self )
		
		
	elseif( msg.name == "checkInventory" ) then
		CheckInventoryItems( self )	
		
		
	elseif( msg.name == "checkInstrumentEquippables" ) then
		CheckInstrumentEquippables( self )
		
    end
    
end




--------------------------------------------------------------
-- have the player stop playing the instrument
--------------------------------------------------------------
function StopPlayingInstrument( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then       
		return
	end
	
	
	-- stop checking to see if the player finished playing the instrument
	self:SetVar( "bBeingPlayed", false )
	
	
	if ( IsLocalPlayerActive( self ) == true ) then
	
		-- stop keeping track of movement key presses
		TrackRecentMovementKeys( self, false )
		
		
		-- make sure the cinematic is stopped
		player:EndCinematic{ leadOut = 1.0 }
		
		
		-- don't want to hear this instrument anymore
		AffectMusic( self, self:GetLOT().objtemplate, false )
		
		
		-- cancel timers that may be running
		GAMEOBJ:GetTimer():CancelTimer("CheckOnPlayer", self)
		GAMEOBJ:GetTimer():CancelTimer("DecreaseImagination", self)
		GAMEOBJ:GetTimer():CancelTimer("checkAnim", self)
		GAMEOBJ:GetTimer():CancelTimer("checkInventory", self)
		GAMEOBJ:GetTimer():CancelTimer("checkInstrumentEquippables", self)
		
	end
		
		
	-- show the player smashing the instrument
	-- and plan when to unequip any equippables for this instrument
	PlaySmashAnim( self, player )
	
end





--------------------------------------------------------------
-- start a timer for when to check whether the player is still playing the instrument
--------------------------------------------------------------
function StartUpdateTimer( self )

	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end

	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["UPDATE_FREQUENCY"], "CheckOnPlayer", self )
end




--------------------------------------------------------------
-- start a timer for when to decrease the player's imagination
--------------------------------------------------------------
function StartImaginationCostTimer( self )

	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end

	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["INSTRUMENT_COST_FREQUENCY"], "DecreaseImagination", self )
end




--------------------------------------------------------------
-- check whether the intrument is still being played
-- this stops if the player runs out of imagination or takes control by moving
--------------------------------------------------------------
function UpdateBeingPlayed( self )

	if ( self:GetVar( "bBeingPlayed" ) == false ) then
		return
	end
	

	-- check whether the player as run out of imagination
	local bOutOfImagination = IsPlayerOutOfImagination( self )

	
	-- if not, check whether the player hit a movement key
	local bMovement = false
	if ( bOutOfImagination == false ) then
		bMovement = DidPlayerHitMovementKey( self )
	end
	
	-- if the player has run out of imagination or hit a movement key, then kick them off the instrument
	if ( bOutOfImagination or bMovement ) then

		self:FireEventServerSide{ args = "stopPlaying", senderID = player }
		


	-- otherwise, set a new timer to check again
	else
		StartUpdateTimer( self )
	end
	
end




--------------------------------------------------------------
-- returns whether the player has run out of imagination
--------------------------------------------------------------
function IsPlayerOutOfImagination( self )

	return ( GetPlayersImagination( self ) <= 0 )
end




--------------------------------------------------------------
-- returns how much imagination the player currently has
--------------------------------------------------------------
function GetPlayersImagination( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
		return 0
	else
		return player:GetImagination{}.imagination
	end
end




--------------------------------------------------------------
-- returns the ID of the active player
--------------------------------------------------------------
function GetActivePlayer( self )

	return getObjectByName(self, "activePlayer")
end




--------------------------------------------------------------
-- takes some imagination away from the player
--------------------------------------------------------------
function DecreasePlayersImagination( self )

	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end
	

	-- get how much imagination the player has now
	local iOldAmount = GetPlayersImagination( self )
	
	
	-- subtract the cost of using the instrument
	local iNewAmount = iOldAmount - CONSTANTS["INSTRUMENT_IMAGINATION_COST"]
	if ( iNewAmount < 0 ) then
		iNewAmount = 0
	end
	
	
	-- update their imagination
	local player = GetActivePlayer( self )
	if ( player ~= nil ) then
		player:SetImagination{ imagination = iNewAmount }
	end
	
	-- start a new timer for when to decrease their imagination next
	StartImaginationCostTimer( self )
	
end




--------------------------------------------------------------
-- returns whether the player hit one of the movement keys
--------------------------------------------------------------
function DidPlayerHitMovementKey( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
		return false
	end
	
	if ( IsLocalPlayerActive( self ) == false ) then
		return false
	end
			
	
	local keysMsg = player:GetRecentMovementKeys{}
	
	return ( keysMsg.bForwardPressed or
		keysMsg.bReversePressed or
		keysMsg.bLeftPressed or
		keysMsg.bRightPressed or
		keysMsg.bJumpPressed )
	
end




--------------------------------------------------------------
-- takes away or gives back player control
-- (pass in false to take it away, true to turn it back on)

-- Make sure you match calls to this carefully because the "PUSH" and "POP" sent to SetStunned
	-- increment or decrement the control's corresponding variable.
-- (Each "true" sent to SetStunned below is used to know which controls' variables to adjust.)
-- You can see how this works at LwoGame \ source \ LWOControllablePhysComponent.cpp in method msgSetStunned
--------------------------------------------------------------
function SetPlayerControl( self, bRestore )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
		return
	end
	
	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end
	
	
	-- note: we can't use SetUserCtrlCompPause because
	-- we still need to receive player movement values in order to know
	-- if they want to break out of the instrument before running out of imagination.
	-- So we use SetStunned instead.
	
	-- SetStunned also lets the player move the camera up and down,
	-- which is consistent with what is allowed while they buid the quickbuild.
	
	local eChangeType = "PUSH"
	if ( bRestore == true ) then
		eChangeType = "POP"
	end
	
	player:SetStunned{ StateChangeType = eChangeType,
					bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }
							
end





--------------------------------------------------------------
-- check whether the active player is also the local player
--------------------------------------------------------------
function IsLocalPlayerActive( self )
	
	local player = GetActivePlayer( self )
	
	return ( player ~= nil and
		self:GetVar( "activePlayer" ) == GAMEOBJ:GetLocalCharID() )

end






--------------------------------------------------------------
-- tell the player's LWOUserControlComponent to start or stop tracking recent presses.
-- When off, it only knows which key presses occurred that frame.
-- If we turn on this tracking, it will rememeber key presses long enough for us to send a query several frames later and
	-- still find out if they key was pressed recently.
	-- We need to use this since our timer allows us to send queries every so often - not every frame.
--------------------------------------------------------------
function TrackRecentMovementKeys( self, bTrack )
	
	local player = GetActivePlayer( self )
	if ( player == nil ) then
		return
	end

	
	-- only let the active player turn his own tracking on or off
	if ( IsLocalPlayerActive( self ) == false ) then
		return
	end
	

	player:TrackRecentMovementKeys{ bTrackForward = bTrack, bTrackReverse = bTrack,
									bTrackLeft= bTrack, bTrackRight = bTrack, bTrackJump = bTrack }
	

end






--------------------------------------------------------------
-- unequip the instrument equippables from the player
--------------------------------------------------------------
function UnequipEquippables( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
		return
	end

	-- if this instrument added any equippables, unequip them and
	-- start a timer to keep checking when they're gone and the inventory items can be re-equipped
	local bInstrumentEquippables = false
	
	if ( self:GetVar( "equippableLeft" ) ~= nil ) then
		getObjectByName( self, "equippableLeft" ):NotifyObject{ name = "unequip" }
		bInstrumentEquippables = true
	end
 
	if ( self:GetVar( "equippableRight" ) ~= nil ) then
		getObjectByName( self, "equippableRight" ):NotifyObject{ name = "unequip" }
		bInstrumentEquippables = true
	end
	
	
	if ( bInstrumentEquippables ) then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "checkInstrumentEquippables", self )
	end
		
end





--------------------------------------------------------------
-- once the anim of the player smashing the instrument finishes, forget about our active player
--------------------------------------------------------------
function EndActivePlayer( self )
	
	-- reset player's anim so they'll do idles again
	ResetPlayersAnim( self )
	
	
	-- re-equip any inventory items the player had in-hand before playing the instrument
	local player = GetActivePlayer( self )
	if ( player ~= nil ) then
		storeObjectByName( self, "reequipPlayer", player )
	end
	ReequipInventoryItems( self )

	
	-- forget about the active player
	self:SetVar( "activePlayer", nil )
	self:SetVar( "checkingInstrumentAnim", false )
	self:SetVar( "targetPositionX", nil )
	self:SetVar( "targetPositionZ", nil )
	
	
	-- break the quickbuild
	self:FireEventServerSide{ senderID = self, args = "reset" }
	
end





--------------------------------------------------------------
-- show the anim of the player smashing the instrument
--------------------------------------------------------------
function PlaySmashAnim( self, player )
	
	-- get the name of the anim
	local instrumentLOT = self:GetLOT().objtemplate
	local szAnimName = CONSTANTS["INSTRUMENT_SMASH_ANIM"][instrumentLOT]

	if ( szAnimName ~= -1 ) then

		if ( IsLocalPlayerActive( self ) == true ) then
		
			-- play the animation
			self:FireEventServerSide{ senderID = player, args = "playSmashAnim", param1 = instrumentLOT }
		end
		
		
		-- find out how long the anim is
		local animLength = player:GetAnimationTime{ animationID = szAnimName }.time
		
		
		-- set a timer for when to reset the quickbuild
		GAMEOBJ:GetTimer():AddTimerWithCancel( animLength, "EndPlayer", self )
		
	end
	
	
	-- unequip equippable items and restore player control
	-- or set a timer to do it partway through the smash anim
	PlanWhenToUnequipAndRestorePlayerControl( self, instrumentLOT )
	
	
	-- if there is no smashing anim, reset the quickbuild now
	if ( szAnimName == -1 ) then
		EndActivePlayer( self )
	end

end





--------------------------------------------------------------
-- hide the completed quickbuild, if appropriate for this instrument
	-- the guitar and bass need to be hidden because the equippable item shows the whole instrument
	-- the drums do not because their equippables are just the drumsticks
	-- the keyboard does not because it has no equippables
--------------------------------------------------------------
function HideQuickbuild( self, instrumentLOT )

	if ( CONSTANTS["INSTRUMENT_HIDE"][instrumentLOT] == false ) then
		return
	end


	self:SetVisible{ visible = false, fadeTime = 0.0 }

end





--------------------------------------------------------------
-- move the player to the best looking position to play this particular instrument
--------------------------------------------------------------
function RepositionPlayer( self, instrumentLOT )

    -- get the quickbuild's position and rotation     
	local qbPos = self:GetPosition().pos
    local qbRot = self:GetRotation()
    
    local playerPos = qbPos
    local playerRot = qbRot
    
    
	-- special case: the player pos needs to be adjusted a little for the drums to put the player behind the drumset
	-- and for the guitar and bass because they are located right on the edge of the state in Happy Flower
	if ( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_DRUM"] ) then
		local drumOffset = { x = 0.0, y = 0.0, z = -0.5 }
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = drumOffset }.newPosition
		
	elseif( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_BASS"] ) then
		local drumOffset = { x = 5.0, y = 0.0, z = 0.0 }
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = drumOffset }.newPosition
		
	elseif( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_GUITAR"] ) then
		local drumOffset = { x = 5.0, y = 0.0, z = 0.0 }
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = drumOffset }.newPosition
		
    end
	
	

	local player = GetActivePlayer( self )
	if ( player ~= nil and IsLocalPlayerActive( self ) == true ) then

		player:SetPosition{ pos = playerPos }
		player:SetRotation{ w = playerRot.w, x = playerRot.x, y = playerRot.y, z = playerRot.z }
		--player:Teleport{ pos = playerPos, bSetRotation = true, w = playerRot.w, x = playerRot.x, y = playerRot.y, z = playerRot.z }
	end
	
	
	
	-- special case: the player's rotation needs to be adjusted a little for the keyboard
	if ( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] ) then

		local newRot = self:GetPosition{}.pos
		newRot.x = 0.0
		newRot.y = 1.5708
		newRot.z = 0.0
		player:RotateObject{ rotation = newRot }
		
    end
    
    
    self:SetVar( "targetPositionX", playerPos.x )
    self:SetVar( "targetPositionZ", playerPos.z )
    

end





--------------------------------------------------------------
-- ask the resource manager to load the corresponding equippable item(s) onto the player
	-- guitar and bass: equip the whole instrument
	-- drums: equip a drumstick into each hand
	-- keyboard: no equippables
	
-- once an equippable is ready, L_AG_CONCERT_INSTRUMENT_EQUIPPABLE_CLIENT.lua will equip it to the player
--------------------------------------------------------------
function LoadEquippables( self, instrumentLOT )
	
	if ( self:GetVar( "activePlayer" ) == nil ) then
		return
	end
	
	
	local leftLOT = CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][instrumentLOT]
	local rightLOT = CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][instrumentLOT]

				
	local config = { { "equip", true },										-- CONFIG_EQUIP in lwoCommon \ include \ LwoConfigData.h
					{ "owner", "|" .. self:GetVar( "activePlayer" ) } }		-- CONFIG_OWNER
	
	
	if ( leftLOT ~= -1 ) then
	
		RESMGR:LoadObject { objectTemplate = leftLOT, 
							owner = self,
							rw = 1.0,
							bIsLocalPlayer = false,
							bDroppedLoot = false,
							configData = config }
	end
	
	
	if ( rightLOT ~= -1 ) then
	
		RESMGR:LoadObject { objectTemplate = rightLOT, 
							owner = self,
							rw = 1.0,
							bIsLocalPlayer = false,
							bDroppedLoot = false,
							configData = config }
	end

		
end





--------------------------------------------------------------
-- checks whether the given LOT is the left hand equippable for this instrument
--------------------------------------------------------------
function IsValidLeftEquippable( self, LOT )
	
	if ( LOT == -1 ) then
		return false
	end
	
	
	local instrumentLOT = self:GetLOT().objtemplate
	return ( LOT == CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][instrumentLOT] )
	
end





--------------------------------------------------------------
-- checks whether the given LOT is the right hand equippable for this instrument
--------------------------------------------------------------
function IsValidRightEquippable( self, LOT )
	
	if ( LOT == -1 ) then
		return false
	end
	
	
	local instrumentLOT = self:GetLOT().objtemplate
	return ( LOT == CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][instrumentLOT] )
	
end




--------------------------------------------------------------
-- child loaded (the equippables)
--------------------------------------------------------------
function onChildLoaded( self, msg )

	local childLOT = msg.childID:GetLOT().objtemplate
	

	-- remember the object ID of this equippable so we can easily unequip later
    if ( IsValidLeftEquippable( self, childLOT ) ) then

		storeObjectByName( self, "equippableLeft", msg.childID )
		
	elseif ( IsValidRightEquippable( self, childLOT ) ) then
	
		storeObjectByName( self, "equippableRight", msg.childID )
            
    end

end






--------------------------------------------------------------
-- reset the object ID's of the equippable items
--------------------------------------------------------------
function ResetEquippableIDs( self )

	self:SetVar( "equippableLeft", nil )
	self:SetVar( "equippableRight", nil )
	
end





--------------------------------------------------------------
-- tell the player to play idles again
--------------------------------------------------------------
function ResetPlayersAnim( self )

	local player = GetActivePlayer( self )
	if ( player ~= nil ) then
	
		player:ResetPrimaryAnimation{}
		player:ResetSecondaryAnimation{}
	end

end




--------------------------------------------------------------
-- unequip equippable items and restore player control
-- or set a timer to do it partway through the smash anim
--------------------------------------------------------------
function PlanWhenToUnequipAndRestorePlayerControl( self, instrumentLOT )

	-- if this instrument has equippable items, set a timer for when to unequip them
	-- for the bass and guitar, the amount of time to wait depends on when during the smash anim it would look best
	-- unequip and restore control now for the drums, which has no smash anim
	-- the keyboard has no equippables, but restore player control now
	
	local unequipTime = CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][instrumentLOT]
	
	if ( unequipTime == 0 ) then
		SetPlayerControl( self, true )
		UnequipEquippables( self )
		
	elseif ( unequipTime == -1 ) then
		SetPlayerControl( self, true )
		
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( unequipTime, "Unequip", self )
	end
	
end




--------------------------------------------------------------
-- notification received from server-side script
--------------------------------------------------------------
function onNotifyClientObject( self, msg )
	
	if ( msg.name == "stopPlaying" ) then
		StopPlayingInstrument( self )
		
		
	elseif ( msg.name == "loadEquippables" ) then
		LoadEquippables( self, self:GetLOT().objtemplate )
		
		
	elseif ( msg.name == "playerLeft" ) then
		if ( self:GetVar( "activePlayer" ) == msg.paramObj:GetID() ) then
			self:SetVar( "bBeingPlayed", false )
			self:SetVar( "activePlayer", nil )
			self:FireEventServerSide{ senderID = self, args = "reset" }
			UnequipEquippables( self )
		end
		
	
	elseif ( msg.name == "checkInstrumentAnim" ) then
		self:SetVar( "checkingInstrumentAnim", true )
		CheckInstrumentAnim( self )
		
	end
	
end





--------------------------------------------------------------
-- RepositionPlayer moves the player to wherever looks best for this instrument
-- and starts the anim of playing the instrument,
-- but other clients will see the player walk from the old spot to the new one
-- which makes the walk anim overstrike the instrument anim.
-- check and see if this client now sees the active player at the desired position.
-- if so, show the instrument playing anim 
--------------------------------------------------------------
function CheckInstrumentAnim( self )
	
	if ( self:GetVar( "checkingInstrumentAnim" ) == false ) then
		return
	end
	
	
	
	local player = GetActivePlayer( self )
	if ( player == nil ) then       
		return
	end
	
	
	local currentPos = player:GetPosition().pos
	local targetPosX = self:GetVar( "targetPositionX" )
	local targetPosZ = self:GetVar( "targetPositionZ" )
	
	if ( targetPosX == nil  or targetPosZ == nil  ) then
		return
	end
	
	
	local deltaX = currentPos.x - targetPosX
	local deltaZ = currentPos.z - targetPosZ
	if ( deltaX < 0.0 ) then
		deltaX = 0.0 - deltaX
	end
	if ( deltaZ < 0.0 ) then
		deltaZ = 0.0 - deltaZ
	end
	
	local tolerance = 0.5
	
	
	if ( deltaX < tolerance and deltaZ < tolerance ) then
		self:SetVar( "checkingInstrumentAnim", false )
		self:SetVar( "targetPositionX", nil  )
		self:SetVar( "targetPositionZ", nil  )
		
		local instrumentLOT = self:GetLOT().objtemplate
		player:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_ANIM"][instrumentLOT], fPriority = 4.0 }
	
	
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "checkAnim", self )
	end
		
end






--------------------------------------------------------------
-- before creating the instrument's equippables, check if the player already has inventory items equipped
-- if so, unequip the inventory items and
-- remember them so we can reequip them once done with the instrument
--------------------------------------------------------------
function RememberInventoryItems( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
	end

	
	-- find out the LWOObjectID's of the items in the mini-fig's hands
	local leftObj  = player:GetEquippedItemInfo{ slot = "special_l" }.objectID
	local rightObj = player:GetEquippedItemInfo{ slot = "special_r" }.objectID
		
	
	-- check which hand(s) this instrument needs to add equippables to
	local instrumentLOT = self:GetLOT().objtemplate
	
	local bInventoryItemsEquipped = false
	
	if leftObj:Exists() then
		self:SetVar( "leftInventoryObjectID", "|" .. leftObj:GetID() )
		player:UnEquipInventory{ itemtounequip = leftObj }
		bInventoryItemsEquipped = true
	end
	
	if rightObj:Exists() then
		self:SetVar( "rightInventoryObjectID", "|" .. rightObj:GetID() )
		player:UnEquipInventory{ itemtounequip = rightObj }
		bInventoryItemsEquipped = true
	end
	
	-- if we need to wait for inventory items to unequip,
	-- set up a timer for checking if it's okay to load the instrument equippables.
	-- if there weren't any inventory items equipped, load the instrument equippables now
	if ( bInventoryItemsEquipped ) then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "checkInventory", self )
		
	else
		self:FireEventServerSide{ args = "loadEquippables", senderID = player }
	end
        
end





--------------------------------------------------------------
-- check whether the inventory items are unequipped yet
-- if so, equip the instrument equippables
--------------------------------------------------------------
function CheckInventoryItems( self )

	local player = GetActivePlayer( self )
	if ( player == nil ) then
	end

	
	local leftObj  = player:GetEquippedItemInfo{ slot = "special_l" }.objectID
	local rightObj = player:GetEquippedItemInfo{ slot = "special_r" }.objectID
	
	
	if leftObj:Exists() or rightObj:Exists() then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "checkInventory", self )
	
	else
		local instrumentLOT = self:GetLOT().objtemplate
		self:FireEventServerSide{ args = "loadEquippables", senderID = player }
	end
	
end





--------------------------------------------------------------
-- the player had inventory items in-hand before playing the instrument.
-- re-equip them.
--------------------------------------------------------------
function ReequipInventoryItems( self )

	local player = getObjectByName( self, "reequipPlayer" )
	
	-- only allow the local player to re-equip its own inventory items
	if ( player == nil or
		self:GetVar( "reequipPlayer" ) ~= GAMEOBJ:GetLocalCharID() ) then
		return
	end
	
	
	-- remember the LWOObjectID's of the inventory items that we unequipped earlier
	local leftObj = getObjectByName( self,  "leftInventoryObjectID" )
	local rightObj = getObjectByName( self, "rightInventoryObjectID" )
	
	-- re-equip the left-hand item, if there was one
	if leftObj:Exists() then
		self:FireEventServerSide{ senderID = player, args = "reequipItem", param1 = leftObj }
	end
		
	-- re-equip the right-hand item, if there was one
	if rightObj:Exists() then
		self:FireEventServerSide{ senderID = player, args = "reequipItem", param1 = rightObj }
	end
	
	
	self:SetVar( "leftInventoryObjectID",  nil )
	self:SetVar( "rightInventoryObjectID", nil )	
	self:SetVar( "reequipPlayer", nil )

	
end





--------------------------------------------------------------
-- check whether the insturment's equppibable items are unequipped yet
-- if so, re-equip the previously-equipped inventory items, if any
--------------------------------------------------------------
function CheckInstrumentEquippables( self )

	local player = getObjectByName( self, "reequipPlayer" )
	if ( player == nil ) then
		return
	end
	
	
	-- check whether either instrument equippable is still equipped
	local bStillEquipped = false
	
	if ( self:GetVar( "equippableLeft" ) ~= nil ) then
		local leftItem = getObjectByName( self, "equippableLeft" )
		if ( leftItem:IsItemEquipped{}.isequipped == true ) then
			bStillEquipped = true
		end
	end
	
	if ( self:GetVar( "equippableRight" ) ~= nil ) then
		local rightItem = getObjectByName( self, "equippableRight" )
		if ( rightItem:IsItemEquipped{}.isequipped == true ) then
			bStillEquipped = true
		end
	end
	
	
	if ( bStillEquipped ) then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "checkInstrumentEquippables", self )
	
	else
		ResetEquippableIDs( self )
	end
		
end