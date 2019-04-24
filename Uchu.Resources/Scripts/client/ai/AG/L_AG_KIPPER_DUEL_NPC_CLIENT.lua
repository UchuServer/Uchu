
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')




--------------------------------------------------------------
-- constants
--------------------------------------------------------------

-- the text used for commenting after a transformation, based on which dueling NPC produced it.
-- you can find the text for these in 4_game \ client \ locale \ translation.xml
TALK = {}

TALK[CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"]] = {}
TALK[CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"]][1]				= Localize( "NPC_NIM_AG_KIPPER_SENTINEL_SMACK_1" )
TALK[CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"]][2]				= Localize( "NPC_NIM_AG_KIPPER_SENTINEL_SMACK_2" )
TALK[CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"]][3]				= Localize( "NPC_NIM_AG_KIPPER_SENTINEL_SMACK_3" )
TALK[CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"]][4]				= Localize( "NPC_NIM_AG_KIPPER_SENTINEL_SMACK_4" )

TALK[CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"]] = {}
TALK[CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"]][1]				= Localize( "NPC_NIM_AG_KIPPER_PARADOX_SMACK_1" )
TALK[CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"]][2]				= Localize( "NPC_NIM_AG_KIPPER_PARADOX_SMACK_2" )
TALK[CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"]][3]				= Localize( "NPC_NIM_AG_KIPPER_PARADOX_SMACK_3" )
TALK[CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"]][4]				= Localize( "NPC_NIM_AG_KIPPER_PARADOX_SMACK_4" )





--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )
	
	registerWithZoneControlObject( self )
	
	-- the NPC will display chat bubbles when clicked on
	SetInteractDistance( self, CONSTANTS["KIPPER_NPC_INTERACT_DISTANCE"] )
	self:SetPickType{ ePickType = 14} 	-- PICK_LIST_INTERACTIVE
										-- from enum PICK_LIST_TYPE in lwoCommonVars.h
	InitializeChat( self )
	
end




--------------------------------------------------------------
-- Render Ready
--------------------------------------------------------------
function onRenderComponentReady( self, msg )

	self:PlayAnimation{ animationID = "idle" }
	
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID = self, args = "ActorReadyKipperDuel" }

end





--------------------------------------------------------------
-- remember the chat bubble messages used if the NPC is clicked on
--------------------------------------------------------------
function InitializeChat( self )

	-- you can find the text for these in client \ locale \ translation.xml
	if ( self:GetLOT{}.objtemplate == CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"] ) then
		
		AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SENTINEL_CLICK" ) )
	
	else
	
		AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_PARADOX_CLICK" ) )

	end

end





--------------------------------------------------------------
-- sets the mouse over distance for interactions
--------------------------------------------------------------
function SetInteractDistance( self, dist )

    if ( self and self:Exists() ) then
    
        self:SetVar("interactDistance", dist)
    
    end

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
        
        self:SetVar( type, table )
        
    end        

end




--------------------------------------------------------------
-- handle client use
--------------------------------------------------------------
function onClientUse(self, msg)
	
	-- display random chat
	local texts = self:GetVar( "interactionText" )
	if ( texts and #texts > 0 ) then
		-- get a random text
		local num = math.random( 1, #texts )
		self:DisplayChatBubble{ wsText = texts[num] }
	end
	
end






--------------------------------------------------------------
-- Notification to object
--------------------------------------------------------------	
function onNotifyObject( self, msg )

	if ( msg.name == "TalkSmack" ) then
		TalkSmack( self, msg.param1 )
		
	end
	
end





--------------------------------------------------------------
-- display bragging text because this NPC just finished a transformation
--------------------------------------------------------------	
function TalkSmack( self )
	
	local duelLOT = self:GetLOT{}.objtemplate
	
	
	-- there is more than one possible comment for each dueling NPC
	local numChoices = #TALK[duelLOT]
	
	
	-- randomly decide which of the comments for this NPC to use	
	local commentNum = math.random( 1, numChoices )
	
	
	-- show the text
	self:DisplayChatBubble{ wsText = TALK[duelLOT][commentNum] }
	
end




