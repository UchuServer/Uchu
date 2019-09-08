
require('o_mis')
require('c_AvantGardens')



--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )

	SetInteractDistance( self, CONSTANTS["KIPPER_NPC_INTERACT_DISTANCE"] )

	-- let the mini-map know that this NPC is a mission giver
	self:AddObjectToGroup{ group = "Minimap_MissionGivers" }

	-- remember what the NPC should say after the mission is finished
	InitializeChat( self )	
end




--------------------------------------------------------------
-- sets the mouse over distance for interactions
--------------------------------------------------------------
function SetInteractDistance( self, dist )

    if ( self and self:Exists() ) then
    
        self:SetVar( "interactDistance", dist )
    
    end

end



--------------------------------------------------------------
-- mouseover
--------------------------------------------------------------
function onCursorOn(self, msg)

	-- which cursor to use depends on whether the player has already finished the mission
	
	if ( IsMissionComplete( self ) == true ) then
	
		self:SetPickType{ePickType = 14}	-- PICK_LIST_INTERACTIVE
		
		DisplayChat( self )
		
		
	else
		self:SetPickType{ePickType = 4}		-- PICK_LIST_MISSION_NPC
	end
end




--------------------------------------------------------------
-- returns whether or not the player his completed the mission
--------------------------------------------------------------
function IsMissionComplete( self )

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if ( player == nil or player == 0 ) then
		return false
	end
	

	local missionState = player:GetMissionState{ missionID = CONSTANTS["KIPPER_MISSION_ID"] }.missionState
   
	-- if the mission is either available or active, then use the mission cursor
  
	if ( IsActiveMissionState( missionState ) ) then
		return false
		
	end

	return true
                
end




--------------------------------------------------------------
-- returns whether or not the mission is still in progress ( or not started yet )
--------------------------------------------------------------
function IsActiveMissionState ( state )

	-- returns whether the given mission state should have a mission cursor
	
	
	if ( state == 1 or state == 9 or 		-- LWO_MISSION_STATE_STATE_AVAILABLE
		state == 2  or state == 10 or		-- LWO_MISSION_STATE_STATE_ACTIVE
		state == 4 ) then					-- LWO_MISSION_STATE_READY_TO_COMPLETE

		return true	 
	end
	
	return false
	
end




--------------------------------------------------------------
-- remember the chat bubble messages used if the NPC is clicked on after the mission is finished
--------------------------------------------------------------
function InitializeChat( self )

	-- you can find the text for these in client \ locale \ translation.xml
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_MISSION_GIVER_VIGILANT" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_MISSION_GIVER_REPORTER" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_MISSION_GIVER_NICE_WORK" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_MISSION_GIVER_KNEW_YOU_COULD" ) )

end





--------------------------------------------------------------
-- adds an interaction, possible types include:
--------------------------------------------------------------
-- mouseOverEffect, mouseOverAnim, mouseOverText, 
-- interactionEffect, interactionAnim, interactionText, 
-- proximityEffect, proximityAnim, proximityText, 
--------------------------------------------------------------
function AddInteraction( self, type, action )

    if ( self and self:Exists() ) then
         
        local table = self:GetVar( type )
        
        -- init table if need to
        if ( table == nil ) then
            table = {}
        end
        
        local num = #table + 1
        table[num] = action
        
        self:SetVar(type, table)
        
    end        

end




--------------------------------------------------------------
-- pop up a chat bubble recognizing that the player finished the mission
--------------------------------------------------------------
function DisplayChat( self )

	-- display random chat
	local texts = self:GetVar( "interactionText" )
	if ( texts and #texts > 0 ) then
		-- get a random text
		local num = math.random( 1, #texts )
		self:DisplayChatBubble{ wsText = texts[num] }
	end
	
end




--------------------------------------------------------------
-- handle client use (click)
--------------------------------------------------------------
function onClientUse( self, msg )
	
	if ( IsMissionComplete( self ) == false ) then
		return
	end
	
	self:PlayAnimation{ animationID = "salute" }
	
end

