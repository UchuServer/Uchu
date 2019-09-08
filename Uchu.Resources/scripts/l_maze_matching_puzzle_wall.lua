
require('o_mis')

CONSTANTS = {}
CONSTANTS["NUM_ACTIVATED_SWITCHES_REQUIRED"] = 3					--TODO: should be 3
CONSTANTS["BROKEN_WALL_DURATION"] = 10                              -- how long to hide the wall before bringing it back



function onStartup( self )

	--print ( "--------------------------------------------" )
	--print ( "MATCHING PUZZLE onStartup" )
	--print ( "--------------------------------------------" ) 

    ResetFlags( self )
    
    self:SetVar( "isHidden", false )
    
    self:SetVar( "originalX", self:GetPosition{}.pos.x )
    self:SetVar( "originalY", self:GetPosition{}.pos.y )
    self:SetVar( "originalZ", self:GetPosition{}.pos.z )
   
    self:SetVar( "hiddenX", self:GetPosition{}.pos.x )
    self:SetVar( "hiddenY", ( self:GetPosition{}.pos.y ) - 20.0 )
    self:SetVar( "hiddenZ", self:GetPosition{}.pos.z ) 

end





function onFireEvent( self, msg )

	--print ( "--------------------------------------------" )
	--print ( "MATCHING PUZZLE onFireEvent" )
	--print ( "--------------------------------------------" ) 
	
	-- remember each trigger's ID as we get it, so that when any trigger is touched, we can check all 3
	StoreTriggerID( self, msg )	

    CheckWhetherSolved( self )
	
end





 function onTimerDone( self, msg )
 
    if (msg.name == "brokenWallTimer") then
        RestoreWall( self )
    end
 end
    




function StoreTriggerID( self, msg )

	-- the msg contains the id of the trigger that just got a touch or unTouch and that fired the event

	
	if ( msg.args == "switch1On" ) then
		storeObjectByName( self, "trigger1ID", msg.senderID )
	end
	
	if ( msg.args == "switch2On" ) then
		storeObjectByName( self, "trigger2ID", msg.senderID )
	end
	
	if ( msg.args == "switch3On" ) then
		storeObjectByName( self, "trigger3ID", msg.senderID )
	end

end





function ResetFlags( self )
	self:SetVar( "switch1Activated", 0 )
	self:SetVar( "switch2Activated", 0 )
	self:SetVar( "switch3Activated", 0 )
end






function CheckWhetherSolved( self )

    if ( self:GetVar("isHidden") == true ) then
        return
    end

	UpdateAllSwitches( self )
	
	-- count how many switches are activated
    local numActivated = 0
	if ( self:GetVar( "switch1Activated" ) == 1 ) then
	    numActivated = numActivated + 1
    end
 	if ( self:GetVar( "switch2Activated" ) == 1 ) then
	    numActivated = numActivated + 1
    end
 	if ( self:GetVar( "switch3Activated" ) == 1 ) then
	    numActivated = numActivated + 1
    end
	
	if ( numActivated >= CONSTANTS["NUM_ACTIVATED_SWITCHES_REQUIRED"] ) then
		HideWall( self )
	end
end






function UpdateSwitch( self, switch )

	local triggerID = nil
	if ( switch == 1 ) then
		triggerID = getObjectByName( self, "trigger1ID" )
	end
	if ( switch == 2 ) then
		triggerID = getObjectByName( self, "trigger2ID" )
	end
	if ( switch == 3 ) then
		triggerID = getObjectByName( self, "trigger3ID" )
	end
	
	if ( triggerID == nil ) then
		return
	end


	-- look for the players near the trigger
	local objs = triggerID:GetProximityObjects().objects
	
	SetSwitchFlag( self, switch, 0 )
	
	local index = 1
	while index <= table.getn(objs)  do
		local target = objs[index]
		local faction = target:GetFaction()
		
		if ( faction and faction.faction == 1 ) then
		
		    --print ( "--------------------------------------------" )
	        --print ( "MATCHING PUZZLE player at switch " .. switch )
	        --print ( "--------------------------------------------" ) 
	
		    SetSwitchFlag( self, switch, 1 )
            return	    
		end
		
		index = index + 1
	end	

end





function SetSwitchFlag( self, switch, flag )

    if ( flag == 1 ) then
        --print ( "--------------------------------------------" )
        --print ( "MATCHING PUZZLE swtich " .. switch .. " set to " .. flag )
        --print ( "--------------------------------------------" )
    end

	if ( switch == 1 ) then
		self:SetVar( "switch1Activated", flag )
	end
	
	if ( switch == 2 ) then
		self:SetVar( "switch2Activated", flag )
	end
	
	if ( switch == 3 ) then
		self:SetVar( "switch3Activated", flag )
	end
	
end





function UpdateAllSwitches( self )

    ResetFlags( self )
	
	local trigger1 = getObjectByName( self, "trigger1ID" )
	local trigger2 = getObjectByName( self, "trigger2ID" )
	local trigger3 = getObjectByName( self, "trigger3ID" )
	
	
	-- there's no way the puzzle is solved if we haven't gotten an onTouch message from each of the 3 switches yet
	-- (unless we're cheating and allowing fewer than 3 to count as solved.)
	
	-- count how many triggerIDs we've gotten so far
	local numTriggerIDs = 0
	if ( trigger1 ~= nil ) then
		numTriggerIDs = numTriggerIDs + 1
	end
	if ( trigger2 ~= nil ) then
		numTriggerIDs = numTriggerIDs + 1
	end
	if ( trigger3 ~= nil ) then
		numTriggerIDs = numTriggerIDs + 1
	end
	
	
	--print ( "------------------------------------------------------" )
	--print ( "MATCHING PUZZLE knows " .. numTriggerIDs .. " trigger IDs" )
	--print ( "------------------------------------------------------" ) 
	        
	
	if ( numTriggerIDs < CONSTANTS["NUM_ACTIVATED_SWITCHES_REQUIRED"] ) then
		return
	end
	
	
	
	-- for each trigger whose ID we know, check whether there's a player there
	
	if ( trigger1 ~= nil ) then
		UpdateSwitch( self, 1 )
	end

	if ( trigger2 ~= nil ) then
		UpdateSwitch( self, 2 )
	end
	
	if ( trigger3 ~= nil ) then
		UpdateSwitch( self, 3 )
	end
	
end






function HideWall( self )

    if ( self:GetVar( "isHidden") == true ) then
        return
    end

    print ( "--------------------------------------------" )
	print ( "MATCHING PUZZLE solved!" )
	print ( "--------------------------------------------" ) 
	
	self:SetVar( "isHidden", true )
    
    -- move the wall down below ground
    local hidePos = self:GetPosition{}.pos
    hidePos.x = self:GetVar( "hiddenX" )
    hidePos.y = self:GetVar( "hiddenY" )
    hidePos.z = self:GetVar( "hiddenZ" )
    self:SetPosition{ pos = hidePos }
    
    -- set a timer for when to put it back
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["BROKEN_WALL_DURATION"], "brokenWallTimer", self )

end






function RestoreWall( self )

    --print ( "--------------------------------------------" )
	--print ( "MATCHING PUZZLE RestoreWall" )
	--print ( "--------------------------------------------" ) 
    
    -- put the wall back up where it belongs
    local showPos = self:GetPosition{}.pos
    showPos.x = self:GetVar( "originalX" )
    showPos.y = self:GetVar( "originalY" )
    showPos.z = self:GetVar( "originalZ" )
    self:SetPosition{ pos = showPos }
    
    self:SetVar( "isHidden", false )
    
    CheckWhetherSolved( self )

end
