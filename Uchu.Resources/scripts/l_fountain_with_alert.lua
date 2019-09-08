
-- REFACTORED SEPTEMBER 23, 2008.  NO LONGER USED.

--[[


-- Note in case we ever want to copy the Youreeka fountain and spout scripts to use in another level:
-- The server-side zone script also does some of the work
	-- It watches for the fountain and all 3 spouts to load
	-- After any one of them loads, it checks to see if it has all 4 yet
		-- If so, then it calls the fountain's onObjectLoaded 3 times to send it each spout ID
		-- and calls each spout's onObjectLoaded to give it the fountain's ID
	-- See L_SKUNK_EVENT.lua functions onObjectLoaded, SendSpoutIDsToFountain, and AreAllFountainPiecesLoaded to see how it works


require('o_mis')


SPOUTS = {}


CONSTANTS = {}
CONSTANTS["SpoutLOT"] = 3283		    		-- the LOT for the spouts


-- states for which spouts are plugged
CONSTANTS["SPOUTS_PLUGGED_NONE"] = 0
CONSTANTS["SPOUTS_PLUGGED_1"] = 1
CONSTANTS["SPOUTS_PLUGGED_2"] = 2
CONSTANTS["SPOUTS_PLUGGED_3"] = 3
CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] = 4
CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] = 5
CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] = 6
CONSTANTS["SPOUTS_DEACTIVATED"] = 7
		
CONSTANTS["NUM_SPOUTS"] = 3						-- how many fountain spouts there are



function onStartup(self)

    -- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
	self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_NONE"] )
	
    self:PlayAnimation{ animationID = "normal" }

end




function onObjectLoaded( self, msg )

	if ( msg.templateID == CONSTANTS["SpoutLOT"] ) then
		-- store object in the spouts array
		local nextSpout = #SPOUTS + 1
        SPOUTS[nextSpout] = msg.objectID:GetID()
        
        --print( "---------------------FOUNTAIN: found a spout" )
       
		local spout = GAMEOBJ:GetObjectByID( SPOUTS[nextSpout] )
		--spout:NotifyObject{ name="test_spout" }
		spout:SetVar( "spoutIndex", nextSpout )
		spout:NotifyObject{ name = "storeIndex", param1 = nextSpout }
	end
	
end





function onNotifyObject( self, msg )

    if ( msg.name == "test_fountain" ) then
	
		--print ( "----------------------------------------" )
		--print ( "FOUNTAIN: OnNotifyObject test_fountain" )
		--print ( "----------------------------------------" )
		
		
	elseif( msg.name == "spout1Plugged" ) then
		Spout1Plugged( self )
		
	elseif( msg.name == "spout2Plugged" ) then
		Spout2Plugged( self )
		
	elseif( msg.name == "spout3Plugged" ) then
		Spout3Plugged( self )
		
	elseif( msg.name == "spout1Unplugged" ) then
		Spout1Unplugged( self )
		
	elseif( msg.name == "spout2Unplugged" ) then
		Spout2Unplugged( self )
		
	elseif( msg.name == "spout3Unplugged" ) then
		Spout3Unplugged( self )

	elseif( msg.name == "deactivateSpouts" ) then
		DeactivateSpouts( self )
		
	elseif( msg.name == "reactivateSpouts" ) then
		ReactivateSpouts( self )
		
	end
	
end




function Spout1Plugged( self )

	-- if we already knew this spout was plugged, or if it has a bouncer already, we're allset
	local oldState = self:GetVar( "eSpoutsState" )
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_1"]  or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] ) then
		return
	end
	

	--print ( "-----------------------FOUNTAIN: Spout1Plugged" )
	
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout1:NotifyObject{ name="plugged" }
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_2"] ) then
		spout1:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="launchable" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_3"] ) then
		spout1:NotifyObject{ name="plugged" }
		spout2:NotifyObject{ name="launchable" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] )
	
	end
	
end





function Spout2Plugged( self )

	-- if we already knew this spout was plugged, or if it has a bouncer already, we're allset
	local oldState = self:GetVar( "eSpoutsState" )
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_2"]  or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] ) then
		return
	end
	

	--print ( "-----------------------FOUNTAIN: Spout2Plugged" )
	
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1"] ) then
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="launchable" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_3"] ) then
		spout1:NotifyObject{ name="launchable" }
		spout2:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] )
	
	end
end





function Spout3Plugged( self )

	-- if we already knew this spout was plugged, or if it has a bouncer already, we're allset
	local oldState = self:GetVar( "eSpoutsState" )
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_3"]  or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] ) then
		return
	end
	

	--print ( "-----------------------FOUNTAIN: Spout3Plugged" )
	
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_3"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1"] ) then
		spout2:NotifyObject{ name="launchable" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_2"] ) then
		spout1:NotifyObject{ name="launchable" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] )
	
	end
end





function Spout1Unplugged( self )

	-- if we didn't think this spout was plugged, don't worry about it
	local oldState = self:GetVar( "eSpoutsState" )
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"]  or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] ) then
		return
	end
	

	--print ( "-----------------------FOUNTAIN: Spout1Unplugged" )
	
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_1"] ) then
		spout1:NotifyObject{ name="normal" }
		spout2:NotifyObject{ name="normal" }
		spout3:NotifyObject{ name="normal" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_NONE"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] ) then
		spout1:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_3"] )
	
	end

end





function Spout2Unplugged( self )

	-- if we didn't think this spout was plugged, don't worry about it
	local oldState = self:GetVar( "eSpoutsState" )
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_3"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] ) then
		return
	end

	--print ( "-----------------------FOUNTAIN: Spout2Unplugged" )
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_2"] ) then
		spout1:NotifyObject{ name="normal" }
		spout2:NotifyObject{ name="normal" }
		spout3:NotifyObject{ name="normal" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_NONE"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] ) then
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_3"] )
	
	end
	
end





function Spout3Unplugged( self )

	-- if we didn't think this spout was plugged, don't worry about it
	local oldState = self:GetVar( "eSpoutsState" )
	
	if (  oldState == CONSTANTS["SPOUTS_PLUGGED_NONE"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_2"] or
		oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] ) then
		return
	end
	

	--print ( "-----------------------FOUNTAIN: Spout3Unplugged" )
	
	
	local spout1 = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	local spout2 = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	local spout3 = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout1 == nil or spout2 == nil or spout3 == nil ) then
		return
	end
		
	
	if ( oldState == CONSTANTS["SPOUTS_PLUGGED_3"] ) then
		spout1:NotifyObject{ name="normal" }
		spout2:NotifyObject{ name="normal" }
		spout3:NotifyObject{ name="normal" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_NONE"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] ) then
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1"] )
		
		
	elseif ( oldState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] ) then
		spout1:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2"] )
	
	end
			
end





function onFireEventServerSide( self, msg )

	--print( "-------------FOUNTAIN: onFireEventServerSide" )

	if ( msg.args == "slash" ) then
		DoSlashCommand( self, msg )		
	end
end





function DoSlashCommand( self, msg )

	-- received a slash command to simulate a player getting on or off a spout
	-- param1 is which spout
	-- param2 is on or off
	
	if ( msg.param1 == 1 ) then
	
		if ( msg.param2 == 1 ) then
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 1 on" )
			Spout1Plugged( self )
		else
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 1 off" )
			Spout1Unplugged( self )
		end
	
	elseif ( msg.param1 == 2 ) then
	
		if ( msg.param2 == 1 ) then
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 2 on" )
			Spout2Plugged( self )
		else
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 2 off" )
			Spout2Unplugged( self )
		end
	
	else
		if ( msg.param2 == 1 ) then
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 3 on" )
			Spout3Plugged( self )
		else
			--print( "-------------FOUNTAIN: DoSlashCommand, spout 3 off" )
			Spout3Unplugged( self )
		end
	
	end

end






function onFireEvent( self, msg )
	
	if ( msg.args == "askSpout1State" ) then
		AnswerSpout1State( self, msg )
		
	elseif ( msg.args == "askSpout2State" ) then
		AnswerSpout2State( self, msg )
		
	elseif ( msg.args == "askSpout3State" ) then
		AnswerSpout3State( self, msg )
		
	end
end





function AnswerSpout1State( self )

	-- Spout 1's client-side script has just started up and wants to know what state the fountain is in
		-- so that it can set its water level accordinly.
	-- It sent a message to its server-side script, which has now asked the fountain.
	-- Tell it how high its water should be (via its server-side script).
	
	--print( "-------------FOUNTAIN: AnswerSpout1State" )
	
	local spout = GAMEOBJ:GetObjectByID( SPOUTS[1] )
	if ( spout == nil ) then
		return
	end
	

	local spoutsState = self:GetVar( "eSpoutsState" )
	
	
	if ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout:NotifyObject{ name="normal" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_2"]  or spoutsState == CONSTANTS["SPOUTS_PLUGGED_3"] ) then
		spout:NotifyObject{ name="increased" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] ) then
		spout:NotifyObject{ name="launchable" }
		
	else
		spout:NotifyObject{ name="plugged" }
	end
	
end





function AnswerSpout2State( self )

	-- Spout 2's client-side script has just started up and wants to know what state the fountain is in
		-- so that it can set its water level accordinly.
	-- It sent a message to its server-side script, which has now asked the fountain.
	-- Tell it how high its water should be (via its server-side script).
	
	--print( "-------------FOUNTAIN: AnswerSpout2State" )
	
	local spout = GAMEOBJ:GetObjectByID( SPOUTS[2] )
	if ( spout == nil ) then
		return
	end
	

	local spoutsState = self:GetVar( "eSpoutsState" )
	
	
	if ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout:NotifyObject{ name="normal" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_1"]  or spoutsState == CONSTANTS["SPOUTS_PLUGGED_3"] ) then
		spout:NotifyObject{ name="increased" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] ) then
		spout:NotifyObject{ name="launchable" }
		
	else
		spout:NotifyObject{ name="plugged" }
	end
	
end





function AnswerSpout3State( self )

	-- Spout 3's client-side script has just started up and wants to know what state the fountain is in
		-- so that it can set its water level accordinly.
	-- It sent a message to its server-side script, which has now asked the fountain.
	-- Tell it how high its water should be (via its server-side script).
	
	--print( "-------------FOUNTAIN: AnswerSpout3State" )
	
	local spout = GAMEOBJ:GetObjectByID( SPOUTS[3] )
	if ( spout == nil ) then
		return
	end
	

	local spoutsState = self:GetVar( "eSpoutsState" )
	
	
	if ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_NONE"] ) then
		spout:NotifyObject{ name="normal" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_1"]  or spoutsState == CONSTANTS["SPOUTS_PLUGGED_2"] ) then
		spout:NotifyObject{ name="increased" }
	
	elseif ( spoutsState == CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] ) then
		spout:NotifyObject{ name="launchable" }
		
	else
		spout:NotifyObject{ name="plugged" }
	end
	
end






function DeactivateSpouts( self )

	-- the skunk invasion is happening and the big base is coming up around the fountain
	-- turn off the effects and the functionality of the spouts
	
	--print( "-------------FOUNTAIN: DeactivateSpouts" )
	
	
	self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_DEACTIVATED"] )

	
	for spoutID = 1, CONSTANTS["NUM_SPOUTS"] do
		
		local spout = GAMEOBJ:GetObjectByID(SPOUTS[spoutID])
		
		if ( spout ~= nil ) then
			spout:NotifyObject{ name = "deactivate" }
		end
		
	end	
	
end






function ReactivateSpouts( self )

	-- the skunk invasion just ended
	-- turn the effects and the functionality of the spouts back on
	
	--print( "-------------FOUNTAIN: ReactivateSpouts" )
	

	local spout1 = GAMEOBJ:GetObjectByID(SPOUTS[1])
	local spout2 = GAMEOBJ:GetObjectByID(SPOUTS[2])
	local spout3 = GAMEOBJ:GetObjectByID(SPOUTS[3])
	
	if ( spout1 == nil or spout2 == nil or spout3 == nil) then
		return
	end
	
	
	
	-- check the proximity objects of each spout to see if anyone is currently on it
	local bPlayerOn1 = DoesSpoutHaveAPlayer( self, 1 )
	local bPlayerOn2 = DoesSpoutHaveAPlayer( self, 2 )
	local bPlayerOn3 = DoesSpoutHaveAPlayer( self, 3 )

	
	-- set the spouts state based on which of the spouts were occupied
	-- and tel the spouts to set their effects / bouncer accordingly
	DetermineSpoutsState( self, bPlayerOn1, bPlayerOn2, bPlayerOn3 )


	
	-- tell the spouts to clear their deactivated flags
	for spoutID = 1, CONSTANTS["NUM_SPOUTS"] do
		
		local spout = GAMEOBJ:GetObjectByID(SPOUTS[spoutID])
		
		if ( spout ~= nil ) then
			spout:NotifyObject{ name = "reactivate" }
		end
		
	end
	
end





function DoesSpoutHaveAPlayer( self, spoutIndex )

	if ( spoutIndex <= 0 or spoutIndex > CONSTANTS["NUM_SPOUTS"] ) then
		return false
	end
	
	
	local spout = GAMEOBJ:GetObjectByID(SPOUTS[spoutIndex])
	if ( spout == nil ) then
		return
	end	
	
	
	-- check this spout's proximity objects to see if anyone is currently on it
	
	local objs = spout:GetProximityObjects().objects
	
	local index = 1
	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		if ( faction and faction.faction == 1 ) then	-- it's a player
			
			--print( "-------------FOUNTAIN: found a player at spout " .. spoutIndex )
			return true
		end
		
		index = index + 1
	end
	
	return false
	
end






function DetermineSpoutsState( self, bPlayerOn1, bPlayerOn2, bPlayerOn3 )

	-- we're turning the spouts back on because the skunk event ended
	-- we've already done a proximity check on each to see if there are any players on it
	-- set the spouts state accordingly, and tell each spout its water height
	
	local spout1 = GAMEOBJ:GetObjectByID(SPOUTS[1])
	local spout2 = GAMEOBJ:GetObjectByID(SPOUTS[2])
	local spout3 = GAMEOBJ:GetObjectByID(SPOUTS[3])
	
	if ( spout1 == nil or spout2 == nil or spout3 == nil) then
		return
	end
	
	
	
	if ( bPlayerOn1 == false and 		bPlayerOn2 == false	and	 	bPlayerOn3 == false ) then
		spout1:NotifyObject{ name="normal" }
		spout2:NotifyObject{ name="normal" }
		spout3:NotifyObject{ name="normal" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_NONE"] )		
	
	elseif ( bPlayerOn1 == true and		bPlayerOn2 == false and 	bPlayerOn3 == false ) then
		spout1:NotifyObject{ name="plugged" }
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1"] )

	elseif ( bPlayerOn1 == false and 	bPlayerOn2 == true and 		bPlayerOn3 == false ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="increased" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2"] )
		
	elseif ( bPlayerOn1 == false and 	bPlayerOn2 == false and 	bPlayerOn3 == true ) then
		spout1:NotifyObject{ name="increased" }
		spout2:NotifyObject{ name="increased" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_3"] )
		
	elseif ( bPlayerOn1 == true and 	bPlayerOn2 == true and 		bPlayerOn3 == false ) then
		spout1:NotifyObject{ name="plugged" }
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="launchable" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_2"] )
		
	elseif ( bPlayerOn1 == true and 	bPlayerOn2 == false and 	bPlayerOn3 == true ) then
		spout1:NotifyObject{ name="plugged" }
		spout2:NotifyObject{ name="launchable" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_1_AND_3"] )

	elseif ( bPlayerOn1 == false and 	bPlayerOn2 == true and 		bPlayerOn3 == true ) then
		spout1:NotifyObject{ name="launchable" }
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] )
		
	-- make a special case here in case there are people on all 3 spouts when the skunk invasion ends
	-- immediately bounce the player off one of them
	elseif ( bPlayerOn1 == true and 	bPlayerOn2 == true and 		bPlayerOn3 == true ) then
		spout1:NotifyObject{ name="launchable" }
		spout2:NotifyObject{ name="plugged" }
		spout3:NotifyObject{ name="plugged" }
		self:SetVar( "eSpoutsState", CONSTANTS["SPOUTS_PLUGGED_2_AND_3"] )

	end

end

--]]

