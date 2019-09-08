
---------------------------------------------------------------------------------------------------------
-- Server-side script for Concert instrument Quick Builds.
---------------------------------------------------------------------------------------------------------


--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')




--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self, msg )
	
	-- needs to register with the server-side zone script
	-- in case the player exits, logs out, etc. while playing an instrument.
	-- the server-side zone script receives a PlayerExit msg, which it can then pass on to the instruments
	registerWithZoneControlObject( self )
	
end



--------------------------------------------------------------
-- Event from client-side
--------------------------------------------------------------
function onFireEventServerSide( self, msg )
	
	if ( msg.args == "reset" ) then
		self:RebuildReset{}
		
		
	elseif ( msg.args == "playInstrumentAnim" ) then
		msg.senderID:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_ANIM"][msg.param1], fPriority = 4.0 }
		self:NotifyClientObject{ name = "checkInstrumentAnim" }
		
		
	elseif ( msg.args == "playSmashAnim" ) then
		msg.senderID:PlayAnimation{ animationID = CONSTANTS["INSTRUMENT_SMASH_ANIM"][msg.param1], fPriority = 4.0 }
		

	elseif ( msg.args == "startMusic" ) then
		AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][msg.param1], true )
		storeObjectByName( self, "musicPlayer", msg.senderID )


	elseif ( msg.args == "stopMusic" ) then	
		AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][msg.param1], false )
		self:SetVar( "musicPlayer", nil )
	
	
	elseif ( msg.args == "stopPlaying" ) then
		self:NotifyClientObject{ name = "stopPlaying", paramObj = msg.senderID }

		
	elseif ( msg.args == "loadEquippables" ) then
		self:NotifyClientObject{ name = "loadEquippables", paramObj = msg.senderID }
		
		
	elseif ( msg.args == "reequipItem" ) then
		ReequipInventoryItem( msg.senderID, msg.param1 )
	end
	
end




--------------------------------------------------------------
-- Start or stop the given the music
--------------------------------------------------------------
function AlterMusic( self, szMusicName, bActivate )

	local soundRepeaters = self:GetObjectsInGroup{ group = "ConcertSoundRepeater", ignoreSpawners = true }.objects
	
	for i = 1, table.maxn (soundRepeaters) do      
     
		if ( bActivate ) then
			soundRepeaters[i]:ActivateNDAudioMusicCue{ m_NDAudioMusicCueName = szMusicName }
			
		else
			soundRepeaters[i]:DeactivateNDAudioMusicCue{ m_NDAudioMusicCueName = szMusicName }
		end
		
	end
		
end





--------------------------------------------------------------
-- object notification from server-side zone script
--------------------------------------------------------------
function onNotifyObject( self, msg )
	
	if ( msg.name == "playerExit" ) then
		
		CancelMusicIfPlayerLeft( self, msg.ObjIDSender )
		StopPlayingIfPlayerLeft( self, msg.ObjIDSender )
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
	if ( musicPlayer == nil ) then
		return
	end

	-- if the player that left was the one playing this instrument, stop the music
	if ( self:GetVar( "musicPlayer" ) == exitPlayer:GetID() ) then
		
		local myLOT = self:GetLOT{}.objtemplate
		AlterMusic( self, CONSTANTS["INSTRUMENT_MUSIC"][myLOT], false )
	
	
		-- also, make sure that if the player who left logs back in,
		-- other clients don't see it come in doing the instrument animation
		musicPlayer:PlayAnimation{ animationID = "ben_is_king", fPriority = 4.0 }
		
				
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
-- tell the client-side script to check if the player that left was the one playing this instrument
	-- and if so, forget about them
--------------------------------------------------------------
function StopPlayingIfPlayerLeft( self, exitPlayer )

	self:NotifyClientObject{ name = "playerLeft", paramObj = exitPlayer }
	
end





--------------------------------------------------------------
-- before playing the instrument, any inventory items the player had in-hand were unequipped
-- re-equip the one with the given LWOObjectID now
--------------------------------------------------------------
function ReequipInventoryItem( player, objID )
	
	if objID:Exists() then
		player:EquipInventory{ itemtoequip = objID }
	end	
end




