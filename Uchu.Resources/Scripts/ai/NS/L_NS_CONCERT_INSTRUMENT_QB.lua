---------------------------------------------------------------------------------------------------------
-- Server-side script for Concert instrument Quick Builds.
--
-- updated mrb... 10/19/10 - made table of smashed players to fix equip bug
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
CONSTANTS["INSTRUMENT_SMASH_ANIM"][CONSTANTS["INSTRUMENT_LOT_DRUM"]] 		= "keyboard-smash"		-- has no smashing anim; use keys for now so that audio can attach smash sound

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
CONSTANTS["REBUILD_STATE_OPEN"] = 0
CONSTANTS["REBUILD_STATE_COMPLETED"] = 2
CONSTANTS["REBUILD_STATE_RESETTING"] = 4
-- copied from enum ERebuildChallengeState in lwoGame \ include \ LWORebuild.h
CONSTANTS["REBUILD_STATE_COMPLETED"] = 2		-- Challenge complete
CONSTANTS["UPDATE_FREQUENCY"] = 0.1				-- while playing the instrument, how often we check whether the player hit a movement key to stop playing	


--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self, msg )	
	-- needs to register with the server-side zone script
	-- in case the player exits, logs out, etc. while playing an instrument.
	-- the server-side zone script receives a PlayerExit msg, which it can then pass on to the instruments
    local zControl = GAMEOBJ:GetZoneControlID()
    
    self:SendLuaNotificationRequest{requestTarget = zControl, messageName = "PlayerExit"}
    self:SendLuaNotificationRequest{requestTarget = zControl, messageName = "PlayerResurrected"}
    
    -- from client
    
	self:SetVar( "bBeingPlayed", false )
	self:SetVar( "activePlayer", nil )	
	
    -- reset the object ID's of the equippable items
	self:SetVar( "equippableLeft", nil )
	self:SetVar( "equippableRight", nil )	
	
	self:SetVar( "checkingInstrumentAnim", false )
	self:SetVar( "targetPositionX", nil  )
	self:SetVar( "targetPositionZ", nil  )	
	self:SetVar( "leftInventoryObjectID",  nil )
	self:SetVar( "rightInventoryObjectID", nil )	
	self:SetVar( "reequipPlayer", nil )	
end

--------------------------------------------------------------
-- called anytime the rebuild object's state changes
--------------------------------------------------------------
function onRebuildNotifyState( self, msg )
	local playerID = msg.player:GetID()
	
	if ( msg.iState == CONSTANTS["REBUILD_STATE_RESETTING"] or msg.iState == CONSTANTS["REBUILD_STATE_OPEN"] ) then
		-- clear the active player if the quickbuild state is being reset or not even started yet
		self:SetVar( "activePlayer", nil )
		--print("Clearing active player, state="..msg.iState)
	else
		-- set the active player in all other states
		self:SetVar( "activePlayer", playerID )
        self:SendLuaNotificationRequest{requestTarget = msg.player, messageName = "Die"}
		--print("Setting active player: "..playerID..", state="..msg.iState)
	end	
    
	-- When the Quick Build is completed, the player gets to play it if they have any imagination    
    if ( msg.iState == CONSTANTS["REBUILD_STATE_COMPLETED"] ) and not msg.player:IsDead().bDead then
		storeObjectByName( self, "activePlayer", msg.player )
		QuickBuildWasBuilt( self )	
	end
end

--------------------------------------------------------------
-- Event from client-side
--------------------------------------------------------------
function onFireEventServerSide( self, msg )	
    if ( msg.args == "stopPlaying" ) then	
        StopPlayingInstrument(self)
	end	
end

--------------------------------------------------------------
-- Start or stop the given the music
--------------------------------------------------------------
function AlterMusic( self, szMusicName, bActivate )
	local soundRepeaters = self:GetObjectsInGroup{ group = "Audio-Concert", ignoreSpawners = true }.objects
	
	for i = 1, table.maxn (soundRepeaters) do           
		if ( bActivate ) then
			soundRepeaters[i]:ActivateNDAudioMusicCue{ m_NDAudioMusicCueName = szMusicName }			
		else
			soundRepeaters[i]:DeactivateNDAudioMusicCue{ m_NDAudioMusicCueName = szMusicName }
		end		
	end		
end

function stopPlayerInteraction(self, player)
	if player:Exists() then		
		CancelMusicIfPlayerLeft( self, player )
		StopPlayingInstrument( self )
		
		local activePlayerID = self:GetVar( "activePlayer" )
		-- if there is an active player, check if it is the player who just exited
		if ( activePlayerID ) then
			--print("Player exited: "..msg.playerID:GetID()..", active="..activePlayerID)			
			if ( player:GetID() == activePlayerID ) then
				-- If the build is complete, kill it when the active player exits.  The logic behind 
				-- this decision is that if the player has exited and the QB is complete, they bailed 
				-- while building (or animating post-build) and the server has finally finished building 
				-- and determined the player is gone on the server side. Now the build needs to be 
				-- smashed so a new one will spawn.
				if ( self:GetRebuildState().iState == CONSTANTS["REBUILD_STATE_COMPLETED"] ) then
					self:RequestDie{killerID = self, killType = "VIOLENT"}
				end
			end
		end
	end	
end

--------------------------------------------------------------
-- object notification from server-side zone script
--------------------------------------------------------------
function notifyPlayerExit(self, other, msg)
    if msg.playerID:Exists() then    
	    self:NotifyClientObject{ name = "playerLeft", paramObj = msg.playerID, rerouteID = msg.playerID }		    
		CancelMusicIfPlayerLeft( self, msg.playerID )
		
		local activePlayerID = self:GetVar( "activePlayer" )
		
		-- if there is an active player, check if it is the player who just exited
		if ( activePlayerID ) then
			--print("Player exited: "..msg.playerID:GetID()..", active="..activePlayerID)			
			if ( msg.playerID:GetID() == activePlayerID ) then
				-- If the build is complete, kill it when the active player exits.  The logic behind 
				-- this decision is that if the player has exited and the QB is complete, they bailed 
				-- while building (or animating post-build) and the server has finally finished building 
				-- and determined the player is gone on the server side. Now the build needs to be 
				-- smashed so a new one will spawn.
				if ( self:GetRebuildState().iState == CONSTANTS["REBUILD_STATE_COMPLETED"] ) then
					self:RequestDie{killerID = self, killType = "VIOLENT"}
				end
			end
		end
    end
end

--------------------------------------------------------------
-- object notification from server-side zone script
--------------------------------------------------------------
function notifyDie(self, other, msg)
    if not other:Exists() then return end
    
    -- stop checking to see if the player finished playing the instrument
    self:SetVar( "bBeingPlayed", false )	
    
    -- make sure the cinematic is stopped
    other:EndCinematic{ leadOut = 1.0 }		-- Needs to be on Client
    
    -- don't want to hear this instrument anymore
    AffectMusic( self, self:GetLOT().objtemplate, false )		
    
    -- cancel timers that may be running
    GAMEOBJ:GetTimer():CancelAllTimers(self) 

    CancelMusicIfPlayerLeft( self, other )
    self:NotifyClientObject{ name = "playerLeft", paramObj = other, rerouteID = other }	
 
    self:SendLuaNotificationCancel{requestTarget=other, messageName="Die"}
    GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "resetRebuild", self )
    
    -- get the deadPlayers list and the old inventory LOTs of the player who died 
    local deadPlayers = GAMEOBJ:GetZoneControlID():GetVar("concertDeadPlayers_" .. self:GetLOT().objtemplate) or {}
	local leftLOT = self:GetVar( "leftInventoryObjectID" ) or 0
	local rightLOT = self:GetVar( "rightInventoryObjectID" ) or 0
    
    -- add the new concat string to the table
    table.insert(deadPlayers, (other:GetID() .. "_" .. rightLOT .. "_" ..  leftLOT))
    
    -- set the updated table on the Zone script based on the instrument LOT
    GAMEOBJ:GetZoneControlID():SetVar("concertDeadPlayers_" .. self:GetLOT().objtemplate, deadPlayers)
end

function split(str, pat)
    local t = {}
    -- creates a table of strings based on the passed in pattern   
    string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)

    return t
end 

function string.starts(String,Start)
    -- finds if a string starts with a giving string.
   return string.sub(String,1,string.len(Start))==Start
end

--------------------------------------------------------------
-- object notification from server-side zone script
--------------------------------------------------------------
function notifyPlayerResurrected(self, other, msg)
    if not msg.playerID:Exists() then return end
    
    local deadPlayers = GAMEOBJ:GetZoneControlID():GetVar("concertDeadPlayers_" .. self:GetLOT().objtemplate) or {}
    
    for k,objVars in ipairs(deadPlayers) do
        local playerTable = split(objVars, "_")
        
        if playerTable[1] == msg.playerID:GetID() then            
            -- see if the player is ready yet
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5 , "TryResurrected_" .. k, self )
            
            -- reset animations for the player that died, play the smash anim to clear the instrument anim
            self:NotifyClientObject{ name = "ResetAnim", paramObj = msg.playerID, rerouteID = msg.playerID }
            msg.playerID:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_SMASH_ANIM"][self:GetLOT().objtemplate], fPriority = 2.0 }
            
            break
        end
    end
end

--------------------------------------------------------------
-- a player left the zone
	-- they could have exited via the x, the menu, or slash command
	-- or logged out via the menu or slash command
	-- or gone back to Character Select via the menu
	-- or had a client crash
	-- or the map could have been reloaded
-- check if the player that left was the one playing this instrument
	-- if so, stop the music
--------------------------------------------------------------
function CancelMusicIfPlayerLeft( self, exitPlayer )
	local musicPlayer = getObjectByName( self, "musicPlayer" )
	
	if ( not musicPlayer or not exitPlayer) then
		return
	end
	
	-- if the player that left was the one playing this instrument, stop the music
	if ( self:GetVar( "musicPlayer" ) == exitPlayer:GetID() ) then		
		local myLOT = self:GetLOT{}.objtemplate
		
		AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][myLOT], false )	
		-- also, make sure that if the player who left logs back in,
		-- other clients don't see it come in doing the instrument animation
		musicPlayer:PlayAnimation{ animationID = "ben_is_king", fPriority = 2.0 }				
		self:SetVar( "musicPlayer", nil )
	end	
end

--------------------------------------------------------------
-- a player left the zone
	-- they could have exited via the x, the menu, or slash command
	-- or logged out via the menu or slash command
	-- or gone back to Character Select via the menu
	-- or had a client crash
	-- or the map could have been reloaded
--------------------------------------------------------------
function StopPlayingIfPlayerLeft( self, exitPlayer )
	self:NotifyClientObject{ name = "playerLeft", paramObj = exitPlayer, rerouteID = exitPlayer}	
end

--------------------------------------------------------------
-- takes some imagination away from the player
--------------------------------------------------------------
function DecreasePlayersImagination( self )	
	-- update their imagination
	local player = GetActivePlayer( self )
	
	if ( player ~= nil ) then
        player:ModifyImagination{ amount = -CONSTANTS["INSTRUMENT_IMAGINATION_COST"] }
	end
	
	-- start a new timer for when to decrease their imagination next
	StartImaginationCostTimer( self )	
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
-- start a timer for when to check whether the player is still playing the instrument
--------------------------------------------------------------
function StartUpdateTimer( self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["UPDATE_FREQUENCY"], "CheckOnPlayer", self )
end

--------------------------------------------------------------
-- start a timer for when to decrease the player's imagination
--------------------------------------------------------------
function StartImaginationCostTimer( self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["INSTRUMENT_COST_FREQUENCY"], "DecreaseImagination", self )
end

--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone( self, msg )
	if ( msg.name == "Hide" ) then
		RepositionPlayer( self, self:GetLOT().objtemplate )	
		
        if CONSTANTS["INSTRUMENT_HIDE"][self:GetLOT().objtemplate] then
            self:SetNetworkVar("Hide", true) 
        end
	elseif ( msg.name == "Play" ) then
		PlayInstrument( self )	
	elseif ( msg.name == "resetRebuild" ) then		
        self:RequestDie{killerID = self, killType = "VIOLENT"}
    elseif ( msg.name == "CheckOnPlayer" ) then
        if ( self:GetVar( "bBeingPlayed" ) == false ) then
            return
        end

	    self:NotifyClientObject{ name = "checkMovement", paramObj = GetActivePlayer(self), rerouteID = GetActivePlayer(self)}	
	    
        -- check whether the player as run out of imagination
        local bOutOfImagination = IsPlayerOutOfImagination( self )        
		
        if ( bOutOfImagination ) then
            -- the player has run out of imagination or hit a movement key, then kick them off the instrument
            StopPlayingInstrument(self)
        else
            -- otherwise, set a new timer to check again
            StartUpdateTimer( self )
        end	
    elseif ( msg.name == "DecreaseImagination" ) then
		DecreasePlayersImagination( self )		
	elseif ( msg.name == "EndPlayer" ) then
		EndActivePlayer( self )		
	elseif ( msg.name == "Unequip" ) then
		UnequipEquippables( self )
	elseif ( msg.name == "TryResurrected" ) then
	elseif ( string.starts(msg.name,"TryResurrected") ) then 
        local playerKey = split(msg.name, "_")[2] or 1 -- player key for the table
        -- get the correct player out of the table of dead players
        local concertDeadPlayers = GAMEOBJ:GetZoneControlID():GetVar("concertDeadPlayers_" .. self:GetLOT().objtemplate) or {}
        
        -- check to see if the key is valid
        if not concertDeadPlayers[tonumber(playerKey)] then return end
        
        -- create a table out of the string for the player, rightLOT, leftLOT
        local deadPlayerTable = split(concertDeadPlayers[tonumber(playerKey)], "_")
        
        -- make sure the table is valid
        if not deadPlayerTable[1] then return end
        
        -- get the playerInit
	    local player = GAMEOBJ:GetObjectByID(deadPlayerTable[1])
	    
        if player:Exists() then
            if player:GetStunned().bCanEquip then
                -- unequip concert instrument and requip player's items
                UnequipEquippables( self, true, player )
                -- reequip the player's old items by sending the table
                ReequipInventoryItems(self, true, deadPlayerTable)	
                
                -- remove the player from the dead list
                table.remove(concertDeadPlayers, playerKey)
                
                -- set the updated dead player table on the zone object
                GAMEOBJ:GetZoneControlID():SetVar("concertDeadPlayers_" .. self:GetLOT().objtemplate, concertDeadPlayers)                  
            else            
                -- try to see if the player is ready yet
                GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5 , "TryResurrected_" .. playerKey, self )
	        end            
	    end
	elseif ( msg.name == "AchievementTimer" ) then
        local player = GetActivePlayer( self )
        
        if ( player ) then
            player:UpdateMissionTask{taskType = "complete", value = 602, value2 = 1, target = self}
	    end
	elseif ( msg.name == "AchievementTimer_20" ) then
        local player = GetActivePlayer( self )
        
        if ( player ) then
            player:UpdateMissionTask{taskType = "complete", value = 302, value2 = 1, target = self}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "AchievementTimer", self )
        end
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
	    
    if player:GetMissionState{missionID = 176}.missionState == 2 then
        player:UpdateMissionTask{taskType = "complete", value = 176, value2 = 1, target = self}
    end

	-- stop checking to see if the player finished playing the instrument
	self:SetVar( "bBeingPlayed", false )	
	
    -- stop keeping track of movement key presses
    self:NotifyClientObject{ name = "stopCheckingMovement", paramObj = player, rerouteID = player}	
    
    -- make sure the cinematic is stopped
    player:EndCinematic{ leadOut = 1.0 }		-- Needs to be on Client
    
    -- don't want to hear this instrument anymore
    AffectMusic( self, self:GetLOT().objtemplate, false )		
    
    -- cancel timers that may be running
    GAMEOBJ:GetTimer():CancelAllTimers(self) 
				
	-- show the player smashing the instrument
	-- and plan when to unequip any equippables for this instrument
	-- get the name of the anim
	local instrumentLOT = self:GetLOT().objtemplate
	local szAnimName = CONSTANTS["INSTRUMENT_SMASH_ANIM"][instrumentLOT]

	if ( szAnimName ~= -1 ) then
		player:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_SMASH_ANIM"][instrumentLOT], fPriority = 2.0 }
				
		-- find out how long the anim is
		local animLength = player:GetAnimationTime{ animationID = szAnimName }.time		
		
		-- set a timer for when to reset the quickbuild
		GAMEOBJ:GetTimer():AddTimerWithCancel( animLength, "EndPlayer", self )		
	end	
	
	-- unequip equippable items and restore player control
	-- or set a timer to do it partway through the smash anim
	local unequipTime = CONSTANTS["INSTRUMENT_UNEQUIP_TIME"][instrumentLOT]
	
	if ( unequipTime == 0 ) then
		UnequipEquippables( self )			
	elseif unequipTime ~= -1 then
		GAMEOBJ:GetTimer():AddTimerWithCancel( unequipTime, "Unequip", self )
	end	
	
	-- if there is no smashing anim, reset the quickbuild now
	if ( szAnimName == -1 ) then
		EndActivePlayer( self )
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
    LoadEquippables( self )
    
	-- start keeping track of movement key presses long enough to know if the player wants to break out of playing the instument
    self:NotifyClientObject{ name = "startPlaying", paramObj = player, rerouteID = player}	
		
	-- show the cinematic
	PlayCinematic( self, instrumentLOT )	

	-- play the animation of the mini-fig playing the given instrument
	PlayInstrumentAnim( self, instrumentLOT )    	
    
	-- start the appropriate music
	AffectMusic( self, instrumentLOT, true )

	-- start a timer for when to check whether the instrument is still being played
	StartUpdateTimer( self )	
	
	-- start a timer for decreasing the player's imagination
	StartImaginationCostTimer( self )

	GAMEOBJ:GetTimer():AddTimerWithCancel( 20, "AchievementTimer_20", self )		
end

--------------------------------------------------------------
-- add or remove music according to which instrument someone just started or stopped playing
-- pass true to bActivate to turn the music on, or false to turn it off.
--------------------------------------------------------------
function AffectMusic( self, instrumentLOT, bActivate)
    local szMusicName = CONSTANTS["INSTRUMENT_MUSIC"][instrumentLOT]      
	local player = GetActivePlayer(self)
	
	if ( player ~= nil ) then
	    local instrumentLOT = self:GetLOT().objtemplate		
		
		if ( bActivate ) then
            AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][instrumentLOT], true )
            storeObjectByName( self, "musicPlayer", player )	
		else
            AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][instrumentLOT], false )
            self:SetVar("musicPlayer", false)	
		end
	end	
end

--------------------------------------------------------------
-- play the animation of the mini-fig playing the given instrument
--------------------------------------------------------------
function PlayInstrumentAnim( self, instrumentLOT )	
	-- start the animation
	local player = GetActivePlayer( self )
	
	if ( player ~= nil ) then
		player:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_ANIM"][instrumentLOT], fPriority = 2.0 }
	end	  
end

--------------------------------------------------------------
-- show the camera panning around the concert area -- Needs to be on Client
--------------------------------------------------------------
function PlayCinematic( self, instrumentLOT )   
	local player = GetActivePlayer( self )
	
	if ( player ~= nil ) then		
		local szPathName = CONSTANTS["INSTRUMENT_CINEMATIC"][instrumentLOT]

		player:PlayCinematic { pathName = szPathName }
	end	
end

function QuickBuildWasBuilt( self )	
	-- set timers for when to hide the completed quickbuild and show the player playing the instrument
	-- do this quicky so we don't have to watch the finished quickbuild model to slam into place
	-- but we have to wait slightly first or get overridden by the quickbuild code		
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "Hide", self )		
	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Play", self )
end

--------------------------------------------------------------
-- unequip the instrument equippables from the player
--------------------------------------------------------------
function UnequipEquippables( self, bRespawning, respawnPlayer )
	local player = false
	
	if bRespawning then
	    player = respawnPlayer
	else
        player = GetActivePlayer( self )
    end
	
	if not player then
		return
	elseif not player:Exists() then
        return
    end
        
	-- find out the LWOObjectID of the items in the mini-fig's hands
	local leftObj = player:GetEquippedItemInfo{ slot = "special_l" }.objectID
	local rightObj = player:GetEquippedItemInfo{ slot = "special_r" }.objectID
	
	-- if this instrument added any equippables, unequip them and
	-- start a timer to keep checking when they're gone and the inventory items can be re-equipped
	-- find out the LOT's of the items in the mini-fig's hands	
	if leftObj:Exists() then
		player:UnEquipInventory{itemtounequip = leftObj, bIgnoreCooldown = true  }
		player:RemoveItemFromInventory{eInvType = 4, iObjID = leftObj }
	end
	
	if rightObj:Exists() then
		player:UnEquipInventory{itemtounequip = rightObj, bIgnoreCooldown = true  }
		player:RemoveItemFromInventory{eInvType = 4, iObjID = rightObj }
	end
		
    if not bRespawning then
        -- break the quickbuild
        self:Smash{killerID = player}
    end
end

--------------------------------------------------------------
-- once the anim of the player smashing the instrument finishes, forget about our active player
--------------------------------------------------------------
function EndActivePlayer( self )			
	local player = GetActivePlayer( self )
	
	if ( player ~= nil ) then
		storeObjectByName( self, "reequipPlayer", player )
	end
	
	-- re-equip any inventory items the player had in-hand before playing the instrument
	ReequipInventoryItems( self )
	
	-- forget about the active player
	self:SetVar( "activePlayer", nil )
	self:SetVar( "checkingInstrumentAnim", false )
	self:SetVar( "targetPositionX", nil )
	self:SetVar( "targetPositionZ", nil )
		
	-- reset the quickbuild
    self:RebuildReset{}
		
    self:NotifyClientObject{ name = "stopPlaying", paramObj = player }
end

--------------------------------------------------------------
-- move the player to the best looking position to play this particular instrument
--------------------------------------------------------------
function RepositionPlayer( self, instrumentLOT )
	local player = GetActivePlayer( self )
	
	if not player then return end
	
    -- get the quickbuild's position and rotation     
	local qbPos = self:GetPosition().pos
    local qbRot = self:GetRotation()
    
    local playerPos = qbPos
    local playerRot = qbRot
        
	-- special case: the player pos needs to be adjusted a little for the drums to put the player behind the drumset
	-- and for the guitar and bass because they are located right on the edge of the state in Happy Flower
	if ( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_DRUM"] ) then
		local posOffset = { x = 0.0, y = 0.0, z = -0.5 }
		
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = posOffset }.newPosition
	elseif( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_BASS"] ) then
		local posOffset = { x = 5.0, y = 0.0, z = 0.0 }
		
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = posOffset }.newPosition
	elseif( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_GUITAR"] ) then
		local posOffset = { x = 5.0, y = 0.0, z = 0.0 }
		
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = posOffset }.newPosition
	elseif( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] ) then
		local posOffset = { x = -0.45, y = 0.0, z = 0.75 }
		
		playerPos = self:GetParallelPosition{ referenceObject = self, offset = posOffset }.newPosition
    end
		
    player:SetRotation{ w = playerRot.w, x = playerRot.x, y = playerRot.y, z = playerRot.z }
    player:Teleport{ pos = playerPos, bSetRotation, bIgnoreY = false, w = playerRot.w, x = playerRot.x, y = playerRot.y, z = playerRot.z }
	
	-- special case: the player's rotation needs to be adjusted a little for the keyboard
	if ( instrumentLOT == CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] ) then
		player:OrientToAngle{fAngle = -0.8, bRelativeToCurrent = true}
    end
        
    self:SetVar( "targetPositionX", playerPos.x )
    self:SetVar( "targetPositionZ", playerPos.z )    
end

--------------------------------------------------------------
-- give the player the items and equip them
--------------------------------------------------------------
function LoadEquippables( self )	
    local player = GetActivePlayer(self)
    
	if not player then
		return
	end	
				
	-- find out the ObjectID's of the items in the mini-fig's hands
	local leftObj  = player:GetEquippedItemInfo{ slot = "special_l" }.objectID
    local rightObj = player:GetEquippedItemInfo{ slot = "special_r" }.objectID
		
    -- Unequip any equipped item and save the ObjectID
    if rightObj:Exists() then
		self:SetVar( "rightInventoryObjectID", "|" .. rightObj:GetID() )
		--print("Right Unequip = " ..rightObj:GetID())
		player:UnEquipInventory{ itemtounequip = rightObj, bIgnoreCooldown = true }
    end
	
	if leftObj:Exists() then
		self:SetVar( "leftInventoryObjectID", "|" .. leftObj:GetID() )
		--print("Left Unequip = " ..leftObj:GetID())
	    player:UnEquipInventory{ itemtounequip = leftObj, bIgnoreCooldown = true }
    end
	
    -- find which instruments we need to equip
	local instrumentLOT = self:GetLOT().objtemplate
	local leftInstrumentLOT = CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_LEFT"][instrumentLOT] or -1
	local rightInstrumentLOT = CONSTANTS["INSTRUMENT_EQUIPPABLE_LOT_RIGHT"][instrumentLOT] or -1
	
	-- Now equip instrument where it belongs	
	-- If it is a left instrument
	if leftInstrumentLOT ~= -1 then	                         
        local newItem =  player:AddNewItemToInventory{ showFlyingLoot = false, invType = 4, iObjTemplate = leftInstrumentLOT }
        
		storeObjectByName( self, "equippableLeft", newItem.newObjID )  
        player:EquipInventory{ itemtoequip = newItem.newObjID, bIgnoreCooldown = true }            
    end	
	
    -- If it is a right instrument
    if rightInstrumentLOT ~= -1 then	
        local newItem =  player:AddNewItemToInventory{ showFlyingLoot = false, invType = 4, iObjTemplate = rightInstrumentLOT }
        
		storeObjectByName( self, "equippableLeft", newItem.newObjID )  
        player:EquipInventory{ itemtoequip = newItem.newObjID, bIgnoreCooldown = true }            
    end	
end

--------------------------------------------------------------
-- the player had inventory items in-hand before playing the instrument.
-- re-equip them.
--------------------------------------------------------------
function ReequipInventoryItems( self, bRespawning, respawnPlayerTable )
	local player = false
	
	
	if bRespawning then
	    player = GAMEOBJ:GetObjectByID(respawnPlayerTable[1])	    
	else
        player = getObjectByName( self, "reequipPlayer" )
    end
	
	if not player then
		return
	elseif not player:Exists() or player:IsDead().bDead then
        return
    end
	
	local leftLOT = false
	local rightLOT = false
	
	if bRespawning then	    
        leftObj = GAMEOBJ:GetObjectByID(respawnPlayerTable[2])
        rightObj = GAMEOBJ:GetObjectByID(respawnPlayerTable[3])
	else
        -- remember the LWOObjectId's of the inventory items that we unequipped earlier
        leftObj = getObjectByName( self,  "leftInventoryObjectID" )
        rightObj = getObjectByName( self, "rightInventoryObjectID" )
	end
	
	-- re-equip the left-hand item, if there was one
	if leftObj then
	    if leftObj:Exists() then
		    player:EquipInventory{ itemtoequip = leftObj, bIgnoreCooldown = true  }
		end
	end	
	
	-- re-equip the right-hand item, if there was one
	if rightObj then
	    if rightObj:Exists() then
		    player:EquipInventory{ itemtoequip = rightObj, bIgnoreCooldown = true  }
		end
	end	
	
	self:SetVar( "leftInventoryObjectID", nil  )
	self:SetVar( "rightInventoryObjectID", nil  )	
	self:SetVar( "reequipPlayer", nil )	
end

--------------------------------------------------------------
-- returns the ID of the active player
--------------------------------------------------------------
function GetActivePlayer( self )
    local player = getObjectByName(self, "activePlayer")
    
    if player then 
        if player:Exists() then
	        return player
	    end
	end
	
	return nil
end

--------------------------------------------------------------
-- returns the ID of the active player
--------------------------------------------------------------
function GetActivePlayerID( self )
    local player = GetActivePlayer( self )
	
	if player then
	    return player:GetID()
	end
	
	return nil
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