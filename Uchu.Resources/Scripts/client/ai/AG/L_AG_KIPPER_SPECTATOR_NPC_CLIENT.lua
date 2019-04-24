
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')




--------------------------------------------------------------
-- constants
--------------------------------------------------------------

-- the LOT's of the models produced by the duel transformations
CONSTANTS["VALID_MODEL_LOTS"] = { CONSTANTS["KIPPER_DUEL_MOUSE_LOT"],
								CONSTANTS["KIPPER_DUEL_CAT_LOT"],
								CONSTANTS["KIPPER_DUEL_DOG_LOT"],
								CONSTANTS["KIPPER_DUEL_DRAGON_LOT"],
								CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"],
								CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"],
								CONSTANTS["KIPPER_DUEL_KIPPER_LOT"],
								CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"] }
							


							

-- the text used for commenting on each transformation, based on the model produced.
-- there is more than one possible comment for each.
-- you can find the text for these in 4_game \ client \ locale \ translation.xml
COMMENTARY = {}

COMMENTARY[CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]][1]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_MOUSE_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]][2]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_MOUSE_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]][3]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_MOUSE_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]][4]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_MOUSE_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_CAT_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_CAT_LOT"]][1]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CAT_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_CAT_LOT"]][2]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CAT_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_CAT_LOT"]][3]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CAT_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_CAT_LOT"]][4]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CAT_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_DOG_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_DOG_LOT"]][1]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DOG_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DOG_LOT"]][2]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DOG_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DOG_LOT"]][3]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DOG_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DOG_LOT"]][4]				= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DOG_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]][1]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DRAGON_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]][2]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DRAGON_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]][3]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DRAGON_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]][4]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_DRAGON_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]][1]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_FIRE_ENGINE_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]][2]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_FIRE_ENGINE_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]][3]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_FIRE_ENGINE_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]][4]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_FIRE_ENGINE_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]][1]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_SUBMARINE_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]][2]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_SUBMARINE_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]][3]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_SUBMARINE_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]][4]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_SUBMARINE_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]][1]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_KIPPER_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]][2]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_KIPPER_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]][3]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_KIPPER_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]][4]			= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_KIPPER_4" )

COMMENTARY[CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]] = {}
COMMENTARY[CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]][1]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_ELEPHANT_1" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]][2]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_ELEPHANT_2" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]][3]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_ELEPHANT_3" )
COMMENTARY[CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]][4]		= Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_ELEPHANT_4" )





--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup( self )

	registerWithZoneControlObject( self )
	
	
	-- the NPC will display chat bubbles when clicked on
	SetInteractDistance( self, CONSTANTS["KIPPER_NPC_INTERACT_DISTANCE"] )
	self:SetPickType{ ePickType = 14 }	-- PICK_LIST_INTERACTIVE
										-- from enum PICK_LIST_TYPE in lwoCommonVars.h
	InitializeChat( self )
	
end




--------------------------------------------------------------
-- Render Ready
--------------------------------------------------------------
function onRenderComponentReady( self, msg )

	self:PlayAnimation{ animationID = "idle" }
	
	-- tell the zone script that the spectator NPC is ready
	-- it will pass that info on to L_ZONE_AG_KIPPER_DUEL_CLIENT.lua
	GAMEOBJ:GetZoneControlID():FireEvent{ senderID = self, args = "ActorReadyKipperDuel" }

end




--------------------------------------------------------------
-- remember the chat bubble messages used if the NPC is clicked on
--------------------------------------------------------------
function InitializeChat( self )

	-- you can find the text for these in 4_game \ client \ locale \ translation.xml
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CHAT1" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CHAT2" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CHAT3" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CHAT4" ) )
	AddInteraction( self, "interactionText", Localize( "NPC_NIM_AG_KIPPER_SPECTATOR_CHAT5" ) )

end





--------------------------------------------------------------
-- sets the mouseover distance for interactions
--------------------------------------------------------------
function SetInteractDistance( self, dist )

    if ( self and self:Exists() ) then
    
        self:SetVar( "interactDistance", dist )
    
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
        
        self:SetVar(type, table)
        
    end        

end




--------------------------------------------------------------
-- handle client use
--------------------------------------------------------------
function onClientUse( self, msg )

	-- the player clicked on the spectator
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

	if ( msg.name == "CommentOnTransformation" ) then
		CommentOnTransformation( self, msg.param1 )
		
	end
	
end





--------------------------------------------------------------
-- display commentary about the current transformation
--------------------------------------------------------------	
function CommentOnTransformation( self, modelLOT )
	
	if ( IsValidModelLOT( modelLOT ) == false ) then
		return
	end
	
	
	-- there is more than one possible comment for each transformation
	-- check how many there are for this transformation
	local numChoices = #COMMENTARY[modelLOT]
	
	
	-- randomly decide which of the comments for this model to use	
	local commentNum = math.random( 1, numChoices )
	
	
	-- show the text
	self:DisplayChatBubble{ wsText = COMMENTARY[modelLOT][commentNum] }
	
end





--------------------------------------------------------------
-- check whether the LOT is one of the transformable models
--------------------------------------------------------------	
function IsValidModelLOT( modelLOT )

	for index = 1, #CONSTANTS["VALID_MODEL_LOTS"] do
	
		if ( modelLOT == CONSTANTS["VALID_MODEL_LOTS"][index] ) then
			return true
		end
		
	end

	return false
	
end
