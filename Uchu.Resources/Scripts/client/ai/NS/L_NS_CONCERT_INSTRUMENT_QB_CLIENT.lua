---------------------------------------------------------------------------------------------------------
-- Client-side script for Concert instrument Quick Builds.
-- When an instrument is built, it locks the mini-fig into an imagination-draining animation.
-- This lasts until the player runs out of imagination or moves voluntarily.
-- Players will hear different music depending on which instruments are currently being played.
--
-- updated mrb... 8/23/10 - refactored to not send/recieve so many messages to the server
---------------------------------------------------------------------------------------------------------

--------------------------------------------------------------
-- constants for the musical instruments quickbuilds
--------------------------------------------------------------
local CONSTANTS = {}
-- the object template for each instrument
CONSTANTS["INSTRUMENT_LOT_GUITAR"] 		= 4039
CONSTANTS["INSTRUMENT_LOT_BASS"]		= 4040
CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] 	= 4041
CONSTANTS["INSTRUMENT_LOT_DRUM"] 		= 4042

-- the anim used to show the player playing each instrument
CONSTANTS["INSTRUMENT_ANIM"] = {}
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 			= "guitar"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 				= "bass"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 			= "keyboard"
CONSTANTS["INSTRUMENT_ANIM"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 				= "drums"

-- the anim used to show the player smashing each instrument
CONSTANTS["INSTRUMENT_SMASH_ANIM"] = {}
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= "guitar-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= "bass-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= "keyboard-smash"
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= -1		-- has no smashing anim

-- the music used for each instrument
CONSTANTS["INSTRUMENT_MUSIC"] = {}
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]]			= "Concert_Guitar"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 			= "Concert_Bass"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 		= "Concert_Keys"
CONSTANTS["INSTRUMENT_MUSIC"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 			= "Concert_Drums"

-- path set up through Happy Flower for the cinematic for each instrument
CONSTANTS["INSTRUMENT_CINEMATIC"] = {}
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= "Concert_Cam_G"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= "Concert_Cam_B"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= "Concert_Cam_K"
CONSTANTS["INSTRUMENT_CINEMATIC"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= "Concert_Cam_D"

-- the LOT for the left hand equippable item used for each instrument
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"] = {}
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 	= 4991
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= 4992
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= -1		-- no equippable used
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= 4995

-- the LOT for the right hand equippable item used for each instrument
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"] = {}
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 	= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 		= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 	= -1
CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= 4996

-- whether or not to hide the completed quickbuild while the player is playing the instrument
CONSTANTS["INSTRUMENT_HIDE"] = {}
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 				= true
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 					= true
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 				= false
CONSTANTS["INSTRUMENT_HIDE"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 					= false

-- once the smash anim starts playing, how long to wait before unequipping the instrument
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"] = {}
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_GUITAR"]] 		= 1.033
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_BASS"]] 			= 0.75
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_KEYBOARD"]] 		= -1		-- has no equippables
CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 			= 0			-- has no smash anim, unequip immediately

-- how much imagination is repeatedly drained while the instrument is being played
CONSTANTS["INSTRUMENT_IMAGINATION_COST"] = 2

-- how often imagination is drained while playing the instrument
CONSTANTS["INSTRUMENT_COST_FREQUENCY"] = 4.0				

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
	
	--ResetEquippableIDs( self )
	
	self:SetVar( "checkingInstrumentAnim", false )
	self:SetVar( "targetPositionX", nil  )
	self:SetVar( "targetPositionZ", nil  )	
	self:SetVar( "leftInventoryLOT", -1 )
	self:SetVar( "rightInventoryLOT", -1 )	
	self:SetVar( "reequipPlayer", nil )	
end

--------------------------------------------------------------
-- called anytime the rebuild object's state changes
--------------------------------------------------------------
function onRebuildNotifyState( self, msg )
    if msg.iState == CONSTANTS["REBUILD_STATE_COMPLETED"] then
        self:SetVar("renderReady", true)
    end
end

function onSetVisible(self, msg)
    if msg.visible then
        self:SetVisible{ visible = false, fadeTime = 0.0 }
    end
end

----------------------------------------------------------------
---- timers
----------------------------------------------------------------
function onTimerDone( self, msg )
	if ( msg.name == "Hide" ) then	
	    if self:GetVar("renderReady") then
	        self:SetVisible{ visible = false, fadeTime = 0.0 }
	    else	        
	        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "Hide", self )      
	    end
    end    
end

----------------------------------------------------------------
---- variables serialized from the server
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self, msg)    
    local player = GAMEOBJ:GetControlledID()
    
    for k,v in pairs(msg.tableOfVars) do
        -- check to see if we have the correct message and deal with it
        if k == "Hide" then
	        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Hide", self )      
        end
    end
end

--------------------------------------------------------------  
-- notification received from server-side script
--------------------------------------------------sss------------
function onNotifyClientObject( self, msg )	
	if ( msg.name == "stopPlaying" ) then
		StopPlayingInstrument( self )	
	    SetPlayerControl( self, true )			
	elseif ( msg.name == "startPlaying" ) then
        self:SetVar( "bBeingPlayed", true )	  
        self:SetVar( "activePlayer", msg.paramObj:GetID() )
        
        TrackRecentMovementKeys( self, true )		   		
	    SetPlayerControl( self )
	elseif ( msg.name == "checkMovement" ) then
        UpdateBeingPlayed( self )			
	elseif ( msg.name == "stopCheckingMovement" ) then
        TrackRecentMovementKeys( self, false )		    	
	elseif ( msg.name == "playerLeft" ) then
		if ( self:GetVar( "activePlayer" ) == msg.paramObj:GetID() ) then
			self:SetVar( "bBeingPlayed", false )
			self:SetVar( "activePlayer", nil )
			TrackRecentMovementKeys( self, false )
		end	
	elseif ( msg.name == "ResetAnim" ) then
	    local player = GAMEOBJ:GetControlledID()
	    
		--Reset Players Anims
		player:ChangeIdleFlags{on = 1}
		player:ResetPrimaryAnimation{}
		player:ResetSecondaryAnimation{}
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
		
		--Reset Players Anims
		player:ChangeIdleFlags{on = 1}
		player:ResetPrimaryAnimation{}
		player:ResetSecondaryAnimation{}
	end
end

-------------------------------------------------------------- 
-- check whether the intrument is still being played 
-- this stops if the player runs out of imagination or takes control by moving
--------------------------------------------------------------
function UpdateBeingPlayed( self )
	if ( self:GetVar( "bBeingPlayed" ) == false ) then
		return
	end
	
	-- if not, check whether the player hit a movement key
	local bMovement = DidPlayerHitMovementKey( self )
	
	-- if the player has run out of imagination or hit a movement key, then kick them off the instrument
	if ( bMovement ) then -- bOutOfImagination or 
		self:FireEventServerSide{ args = "stopPlaying", senderID = player }
	end	
end

-------------------------------------------------------------- 
-- returns the ID of the active player
--------------------------------------------------------------
function GetActivePlayer( self )
	return getObjectByName(self, "activePlayer")
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
		self:GetVar( "activePlayer" ) == GAMEOBJ:GetControlledID():GetID() )
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
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(self, varName, object)
    if not object:Exists() or not varName then return end
    
    local idString = object:GetID()
    
    finalID = "|" .. idString
    self:SetVar(varName, finalID) 
end

--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(self, varName)
    if not varName then return end
    
    local targetID = self:GetVar(varName)
    
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end	
end
