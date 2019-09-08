--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_AvantGardens')

--------------------------------------------------------------
-- constants
--------------------------------------------------------------
-- which of the two NPC's is associated with the transformations named 1_ and which with the transformations named 2_
-- this is determined by a flag set in scripts \ c_AvantGardens.lua
if ( CONSTANTS["KIPPER_DUEL_SENTINEL_GOES_FIRST"] == true ) then
	CONSTANTS["ORDER_SENTINEL"] = 1
	CONSTANTS["ORDER_PARADOX"] = 2
else
	CONSTANTS["ORDER_PARADOX"] = 1
	CONSTANTS["ORDER_SENTINEL"] = 2
end

-- after sending the load request for a model, how long to wait before the assemly NPC comments on the transformation
CONSTANTS["COMMENTARY_DELAY"]	= 0.25	

-- the order in which the different models get transformed
-- the number in front represents which of the 2 competing objects changes
TRANSFORMATIONS = {}
TRANSFORMATIONS["1_NOTHING_TO_MOUSE"] 		= 1
TRANSFORMATIONS["2_NOTHING_TO_CAT"] 		= 2
TRANSFORMATIONS["1_MOUSE_TO_DOG"] 			= 3
TRANSFORMATIONS["2_CAT_TO_DRAGON"] 			= 4
TRANSFORMATIONS["1_DOG_TO_FIRE_ENGINE"] 	= 5
TRANSFORMATIONS["2_DRAGON_TO_SUBMARINE"] 	= 6
TRANSFORMATIONS["1_FIRE_ENGINE_TO_KIPPER"]	= 7
TRANSFORMATIONS["2_SUBMARINE_TO_ELEPHANT"] 	= 8
TRANSFORMATIONS["1_KIPPER_TO_MOUSE"] 		= 9
TRANSFORMATIONS["2_ELEPHANT_TO_CAT"] 		= 10
TRANSFORMATIONS["NOT_STARTED"]		 		= 0
TRANSFORMATIONS["START_FROM_NOTHING"] 		= TRANSFORMATIONS["1_NOTHING_TO_MOUSE"] 
TRANSFORMATIONS["FIRST_LOOPING"] 			= TRANSFORMATIONS["1_MOUSE_TO_DOG"]
TRANSFORMATIONS["LAST_LOOPING"] 			= TRANSFORMATIONS["2_ELEPHANT_TO_CAT"]

-- the LOT for the model produced by each transformation
MODEL_LOTS = {}
MODEL_LOTS[TRANSFORMATIONS["1_NOTHING_TO_MOUSE"] ] 		= CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]
MODEL_LOTS[TRANSFORMATIONS["2_NOTHING_TO_CAT"]]  		= CONSTANTS["KIPPER_DUEL_CAT_LOT"]
MODEL_LOTS[TRANSFORMATIONS["1_MOUSE_TO_DOG"]]  			= CONSTANTS["KIPPER_DUEL_DOG_LOT"]
MODEL_LOTS[TRANSFORMATIONS["2_CAT_TO_DRAGON"]] 			= CONSTANTS["KIPPER_DUEL_DRAGON_LOT"]
MODEL_LOTS[TRANSFORMATIONS["1_DOG_TO_FIRE_ENGINE"]]  	= CONSTANTS["KIPPER_DUEL_FIRE_ENGINE_LOT"]
MODEL_LOTS[TRANSFORMATIONS["2_DRAGON_TO_SUBMARINE"]]  	= CONSTANTS["KIPPER_DUEL_SUBMARINE_LOT"]
MODEL_LOTS[TRANSFORMATIONS["1_FIRE_ENGINE_TO_KIPPER"]] 	= CONSTANTS["KIPPER_DUEL_KIPPER_LOT"]
MODEL_LOTS[TRANSFORMATIONS["2_SUBMARINE_TO_ELEPHANT"]]  = CONSTANTS["KIPPER_DUEL_ELEPHANT_LOT"]
MODEL_LOTS[TRANSFORMATIONS["1_KIPPER_TO_MOUSE"]]  		= CONSTANTS["KIPPER_DUEL_MOUSE_LOT"]
MODEL_LOTS[TRANSFORMATIONS["2_ELEPHANT_TO_CAT"]]  		= CONSTANTS["KIPPER_DUEL_CAT_LOT"]

-- which NPC ( 1 or 2 ) is the owner of each transformation
OWNER_NUMS = {}
OWNER_NUMS[TRANSFORMATIONS["1_NOTHING_TO_MOUSE"]]  		= 1
OWNER_NUMS[TRANSFORMATIONS["2_NOTHING_TO_CAT"]]  		= 2
OWNER_NUMS[TRANSFORMATIONS["1_MOUSE_TO_DOG"]]  			= 1
OWNER_NUMS[TRANSFORMATIONS["2_CAT_TO_DRAGON"]]  		= 2
OWNER_NUMS[TRANSFORMATIONS["1_DOG_TO_FIRE_ENGINE"]]  	= 1
OWNER_NUMS[TRANSFORMATIONS["2_DRAGON_TO_SUBMARINE"]]  	= 2
OWNER_NUMS[TRANSFORMATIONS["1_FIRE_ENGINE_TO_KIPPER"]] 	= 1
OWNER_NUMS[TRANSFORMATIONS["2_SUBMARINE_TO_ELEPHANT"]]  = 2
OWNER_NUMS[TRANSFORMATIONS["1_KIPPER_TO_MOUSE"]]  		= 1
OWNER_NUMS[TRANSFORMATIONS["2_ELEPHANT_TO_CAT"]]  		= 2

-- how close to the corresponding NPC to place the model
-- (compared to the whole distance between the 2 NPC's )
CONSTANTS["DISTANCE_FACTOR"]	= 7.0 / 24.0	-- half way between 1/3 of the way and 1/4 of the way

-- the LOTs for the dummy models used to start us off
-- these transform into the mouse and cat
CONSTANTS["KIPPER_DUEL_DUMMY_MOUSE_LOT"] 	= 4966
CONSTANTS["KIPPER_DUEL_DUMMY_CAT_LOT"]		= 4973

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartupKipperDuel(self)     
    self:SetVar("currentTransformation", TRANSFORMATIONS["NOT_STARTED"] )        
end

--------------------------------------------------------------
-- Called when zone object gets an onFireEvent for "ActorReadyKipperDuel"
-- because the render component just became ready for one of the NPC's
--------------------------------------------------------------
function ActorReadyKipperDuel( self, actor )
	local LOT = actor:GetLOT().objtemplate

	if ( LOT == CONSTANTS["KIPPER_DUEL_SENTINEL_NPC_LOT"] ) then
		storeObjectByName( self, "KipperDuelSentinelNPC", actor )		
	elseif ( LOT == CONSTANTS["KIPPER_DUEL_PARADOX_NPC_LOT"] ) then
		storeObjectByName( self, "KipperDuelParadoxNPC", actor )		
	elseif( LOT == CONSTANTS["KIPPER_SPECTATOR_ASSEMBLY_NPC_LOT"] ) then
		storeObjectByName( self, "KipperSpectatorAssemblyNPC", actor )		
	end	
	
	-- if all 3 actors are now ready, set up a timer to set off the whole duel
	if ( VerifyActors( self ) ) then
		SpawnInvisibleStartingModels( self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["KIPPER_DUEL_TIME_BETWEEN_MODELS"], "nextTransformation", self )
	end	
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDoneKipperDuel( self, msg )	
	if ( msg.name == "nextTransformation" ) then
		DoNextTransformation( self )		
	elseif ( msg.name == "commentary" ) then
		ShowSpectatorCommentary( self )	
	end	
end

--------------------------------------------------------------
-- set the transformation number to the next one in the progression
--------------------------------------------------------------
function IncrementTransformationNum( self ) 
	-- check which transformation was done last
	local curTransformation = self:GetVar( "currentTransformation" )	
	
	-- if we haven't started yet, start with the 2 transformations from nothing
	if ( curTransformation == TRANSFORMATIONS["NOT_STARTED"] ) then
		self:SetVar("currentTransformation", TRANSFORMATIONS["START_FROM_NOTHING"] )
	-- if both objects have cycled back to their first incarnations, then restart the chain of transformations
	-- (skip the transformations from nothing)
	elseif ( curTransformation == TRANSFORMATIONS["LAST_LOOPING"] ) then
		self:SetVar("currentTransformation", TRANSFORMATIONS["FIRST_LOOPING"] )	
	-- otherwise, just go to the next transformation in order
	else
		self:SetVar("currentTransformation", curTransformation + 1 )
	end	
end

--------------------------------------------------------------
-- returns whether the stored objects are valid actors for the kipper duel
--------------------------------------------------------------
function VerifyActors(self)
	if ( getObjectByName( self, "KipperDuelSentinelNPC" ) and
		getObjectByName( self, "KipperDuelParadoxNPC" ) and
		getObjectByName( self, "KipperSpectatorAssemblyNPC" ) ) then	
		if ( getObjectByName( self, "KipperDuelSentinelNPC" ):Exists() and
			getObjectByName( self, "KipperDuelParadoxNPC" ):Exists() and
			getObjectByName( self, "KipperSpectatorAssemblyNPC" ):Exists() ) then			
			return true
		end
	end	
	return false
end

--------------------------------------------------------------
-- transform the existing model into a new one
--------------------------------------------------------------
function DoNextTransformation( self )
	if ( VerifyActors(self) == false ) then
		return
	end	
	
	-- figure out which transformation is due
	IncrementTransformationNum( self )	
	--print( "incremented transformation num to " .. self:GetVar("currentTransformation" ) )	
	-- ask the LWOChangelingComponent to transform it into the one we need now
	TransformModel( self )	
end

--------------------------------------------------------------
-- returns whether the current transformation number affects model 1 or 2
--------------------------------------------------------------
function GetChanglingModelNum( self )
	-- return whether the current transformation is on the 1st or 2nd model	
	local curTransformation = self:GetVar( "currentTransformation" )
	
	return OWNER_NUMS[curTransformation]
end

--------------------------------------------------------------
-- returns whether or not the current model belongs to the Sentinel NPC
-- if not, it belongs to the Paradox NPC
--------------------------------------------------------------
function ModelBelongsToSentinelNPC( self )
	local modelNum = GetChanglingModelNum( self )
	
	if ( modelNum == CONSTANTS["ORDER_SENTINEL"] ) then
		return true
	else
		return false
	end	
end

--------------------------------------------------------------
-- have the Assembly NPC comment on the last transformation
--------------------------------------------------------------
function ShowSpectatorCommentary( self )
	if ( VerifyActors(self) == false ) then
		return
	end	
	
	MakeSpectatorWatch( self )
	
	local assemblyObj = getObjectByName( self, "KipperSpectatorAssemblyNPC" )
	local modelLOT = GetCurrentModelLOT( self )	
	
	-- send a notification to the assembly NPC with the LOT for the model to comment on
	-- this goes through to scripts \ client \ ai \ AG \ L_AG_KIPPER_SPECTATOR_NPC_CLIENT.lua
	assemblyObj:NotifyObject{ name = "CommentOnTransformation", param1 = modelLOT }
end

--------------------------------------------------------------
-- get the LOT for the model produced by the current transformation
--------------------------------------------------------------
function GetCurrentModelLOT( self )	
	local curTransformation = self:GetVar( "currentTransformation" )
	
	return MODEL_LOTS[curTransformation]	
end

--------------------------------------------------------------
-- ask the LWOChangelingBuildComponent to transform the current model into the one we want now
--------------------------------------------------------------
function TransformModel( self )
	local SentinelObj = getObjectByName( self, "KipperDuelSentinelNPC" )
	local ParadoxObj = getObjectByName( self, "KipperDuelParadoxNPC" )	
	local targetLOT = GetCurrentModelLOT( self )	
	
	if ( GetChanglingModelNum( self ) == 1 ) then	
		if ( getObjectByName( self, "changlingModel_1" ) ) then			
			if ( ModelBelongsToSentinelNPC( self ) == true ) then
				--print( "sentinel transforms model 1 into " .. targetLOT )
				getObjectByName( self, "changlingModel_1" ):TransformChangelingBuild{ listenerID = self,
																					miniFigID = SentinelObj,
																					newModelLOT = targetLOT,
																					fAdditionalBricksDelay = CONSTANTS["KIPPER_DUEL_ADDITIONAL_BRICKS_DELAY"],
																					fMaxPositionOffset = CONSTANTS["KIPPER_DUEL_MAX_BRICK_POS_OFFSET"] }
			else
				--print( "paradox transforms model 1 into " .. targetLOT )
				getObjectByName( self, "changlingModel_1" ):TransformChangelingBuild{ listenerID = self,
																					miniFigID = ParadoxObj,
																					newModelLOT = targetLOT,
																					fAdditionalBricksDelay = CONSTANTS["KIPPER_DUEL_ADDITIONAL_BRICKS_DELAY"],
																					fMaxPositionOffset = CONSTANTS["KIPPER_DUEL_MAX_BRICK_POS_OFFSET"] }
			end
		end	
	
	else	-- model 2	
		if ( getObjectByName( self, "changlingModel_2" ) ) then
			if ( ModelBelongsToSentinelNPC( self ) == true ) then
				--print( "sentinel transforms model 2 into " .. targetLOT ) 
				getObjectByName( self, "changlingModel_2" ):TransformChangelingBuild{ listenerID = self,
																					miniFigID = SentinelObj,
																					newModelLOT = targetLOT,
																					fAdditionalBricksDelay = CONSTANTS["KIPPER_DUEL_ADDITIONAL_BRICKS_DELAY"],
																					fMaxPositionOffset = CONSTANTS["KIPPER_DUEL_MAX_BRICK_POS_OFFSET"] }
			else
				--print( "paradox transforms model 2 into " .. targetLOT )
				getObjectByName( self, "changlingModel_2" ):TransformChangelingBuild{ listenerID = self,
																					miniFigID = ParadoxObj,
																					newModelLOT = targetLOT,
																					fAdditionalBricksDelay = CONSTANTS["KIPPER_DUEL_ADDITIONAL_BRICKS_DELAY"],
																					fMaxPositionOffset = CONSTANTS["KIPPER_DUEL_MAX_BRICK_POS_OFFSET"] }
			end
		end
	end	
end

--------------------------------------------------------------
-- Called when zone object gets an onFireEvent for "ModelReadyKipperDuel"
-- because the render component became ready for the newest changeling model
--------------------------------------------------------------
function ModelReadyKipperDuel( self, model )
	local LOT = model:GetLOT().objtemplate
	
	if ( IsValidModel( LOT ) == false ) then
		return
	end	

	local modelNum = GetChanglingModelNum( self )
		
	if ( modelNum == 1 ) then
		storeObjectByName( self, "changlingModel_1", model )
	else
		storeObjectByName( self, "changlingModel_2", model )
	end	
	
	StartTimersUponNewModelLoaded( self )	
	DuelingNPCsReactToNewModel( self )	
end

--------------------------------------------------------------
-- return if template is a valid changeling model
--------------------------------------------------------------
function IsValidModel( LOT )
	for index = 1, #MODEL_LOTS do	
		if ( LOT  == MODEL_LOTS[index] ) then
			return true
		end
	end

	return false
end

--------------------------------------------------------------
-- return if template is a valid changeling model
--------------------------------------------------------------
function StartTimersUponNewModelLoaded( self )
	-- start a timer for having the Assembly NPC spectator comment on the transformation
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["COMMENTARY_DELAY"], "commentary", self )	
	
	-- start a new timer to initiate the next transformation
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["KIPPER_DUEL_TIME_BETWEEN_MODELS"], "nextTransformation", self )
end

--------------------------------------------------------------
-- a transformation just finished and the old object sent a msg with the new model's object ID
--------------------------------------------------------------
function onReturnChangelingBuildID( self, msg )
	-- test only	
	-- not actually using this because we want to wait until we know the render component is ready for the new model
	-- that is caught in L_AG_KIPPER_MODEL_CLIENT.lua and passed through to the zone script	
	-- (the return msg is sent is sent at the same time as the spawn msg for the new model)	
	--print( "kipper duel received new model ID" )
end

--------------------------------------------------------------
-- spawn hidden models that can be changed into the mouse and cat to start with
--------------------------------------------------------------
function SpawnInvisibleStartingModels( self )	
	if ( VerifyActors(self) == false ) then
		return
	end	

	-- how close to the corresponding NPC to place the model
	-- (compared to the whole distance between the 2 NPC's )
	local distanceFactor = CONSTANTS["DISTANCE_FACTOR"]
		
	-- get the NPC's positions
	local posSentinel = getObjectByName(self, "KipperDuelSentinelNPC"):GetPosition().pos
	local posParadox = getObjectByName(self, "KipperDuelParadoxNPC"):GetPosition().pos
		
	-- calculate the model positions
	local posForSentinelModel = { x = ( ( posParadox.x - posSentinel.x ) * distanceFactor ) + posSentinel.x,
								y = ( ( posParadox.y - posSentinel.y ) * distanceFactor ) + posSentinel.y,
								z = ( ( posParadox.z - posSentinel.z ) * distanceFactor ) + posSentinel.z }
	local posForParadoxModel = { x = ( ( posSentinel.x - posParadox.x ) * distanceFactor ) + posParadox.x,
								y = ( ( posSentinel.y - posParadox.y ) * distanceFactor ) + posParadox.y,
								z = ( ( posSentinel.z - posParadox.z ) * distanceFactor ) + posParadox.z }	
	
	-- get the NPC's rotations
	-- we'll use the same rotations for the models
	local rotMsg1 = getObjectByName(self, "KipperDuelSentinelNPC"):GetRotation()
	local rotForSentinelModel = { w = rotMsg1.w, x = rotMsg1.x, y = rotMsg1.y, z = rotMsg1.z }	
	local rotMsg2 = getObjectByName(self, "KipperDuelParadoxNPC"):GetRotation()
	local rotForParadoxModel = { w = rotMsg2.w, x = rotMsg2.x, y = rotMsg2.y, z = rotMsg2.z }	

	-- spawn the new models
	RESMGR:LoadObject { objectTemplate = CONSTANTS["KIPPER_DUEL_DUMMY_MOUSE_LOT"],
						bIsSmashable = true,
						x = posForSentinelModel.x,
						y = posForSentinelModel.y,
						z = posForSentinelModel.z,
						rw = rotForSentinelModel.w,
						rx = rotForSentinelModel.x,
						ry = rotForSentinelModel.y,
						rz = rotForSentinelModel.z,
						owner = self }						
	
	RESMGR:LoadObject { objectTemplate = CONSTANTS["KIPPER_DUEL_DUMMY_CAT_LOT"],
						bIsSmashable = true,
						x = posForParadoxModel.x,
						y = posForParadoxModel.y,
						z = posForParadoxModel.z,
						rw = rotForParadoxModel.w,
						rx = rotForParadoxModel.x,
						ry = rotForParadoxModel.y,
						rz = rotForParadoxModel.z,
						owner = self }
end

--------------------------------------------------------------
-- loaded up one of the invisible starting models
--------------------------------------------------------------
function onChildLoadedKipperDuel(self, msg)
	if ( msg.templateID == 4966 ) then
		storeObjectByName( self, "changlingModel_1", msg.childID )	
	elseif ( msg.templateID == 4973 ) then
		storeObjectByName( self, "changlingModel_2", msg.childID )	
	end	
end

--------------------------------------------------------------
-- after a new model is loaded up, the dueling NPC that created it talks it up
--------------------------------------------------------------
function ModelsOwnerTalksSmack( self )
	if ( VerifyActors(self) == false ) then
		return
	end
	
	local duelObj = getObjectByName( self, "KipperDuelParadoxNPC" )
	
	if ( ModelBelongsToSentinelNPC( self ) == true ) then
		duelObj = getObjectByName( self, "KipperDuelSentinelNPC" )
	end			
	
	local modelLOT = GetCurrentModelLOT( self )	
	-- send a notification to the NPC with the LOT for the model to comment on
	-- this goes through to scripts \ client \ ai \ AG \ L_AG_KIPPER_DUEL_NPC_CLIENT.lua
	duelObj:NotifyObject{ name = "TalkSmack", param1 = modelLOT }
end

--------------------------------------------------------------
-- after a new model is loaded up, the dueling NPCs react to it
--------------------------------------------------------------
function DuelingNPCsReactToNewModel( self )
	if ( VerifyActors(self) == false ) then
		return
	end
	
	local sentinelObj = getObjectByName( self, "KipperDuelSentinelNPC" )
	local paradoxObj = getObjectByName( self, "KipperDuelParadoxNPC" )	
	local ownerObj = sentinelObj
	local opponentObj = paradoxObj	
	
	if ( ModelBelongsToSentinelNPC( self ) == false ) then
		ownerObj = paradoxObj
		opponentObj = sentinelObj
	end
		
	-- the owner of the new model plays a gloating anim
	ownerObj:PlayAnimation{ animationID = CONSTANTS["KIPPER_ANIM_GLOAT"], fPriority = 1.5 }	
	-- the other dueling NPC plays a pouting anim
	opponentObj:PlayAnimation{ animationID = CONSTANTS["KIPPER_ANIM_POUT"], fPriority = 1.5 }	
	-- the new model's owner comments on it
	ModelsOwnerTalksSmack( self )
end

--------------------------------------------------------------
-- make the spectator animate in the direction of the transformation
--------------------------------------------------------------
function MakeSpectatorWatch( self )
	if ( VerifyActors(self) == false ) then
		return
	end	
	
	local spectatorObj = getObjectByName( self, "KipperSpectatorAssemblyNPC" )	
	local bSentinelIsOwner = ModelBelongsToSentinelNPC( self )
	local bSentinelIsToTheLeft = CONSTANTS["SENTINEL_IS_LEFT_OF_SPECTATOR"]	
	local bLookLeft = true
	
	if ( bSentinelIsOwner == true and bSentinelIsToTheLeft == false ) then
		bLookLeft = false	
	elseif ( bSentinelIsOwner == false and bSentinelIsToTheLeft == true ) then
		bLookLeft = false
	end	
	
	if ( bLookLeft ) then
		spectatorObj:PlayAnimation{ animationID = CONSTANTS["KIPPER_ANIM_LEFT"], fPriority = 1.5 }
	else
		spectatorObj:PlayAnimation{ animationID = CONSTANTS["KIPPER_ANIM_RIGHT"], fPriority = 1.5 }
	end	
end
