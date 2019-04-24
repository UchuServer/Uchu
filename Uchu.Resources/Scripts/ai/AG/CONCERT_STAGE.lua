
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require ('o_mis')
require('c_AvantGardens')
require('o_ChoicebuildBonus')	-- determines when all 4 choicebuilds match



--------------------------------------------------------------
-- constants
--------------------------------------------------------------

-- all 4 LOT's
CONSTANTS["CHOICEBUILD_LOTS"] = {}
CONSTANTS["CHOICEBUILD_LOTS"][1] = CONSTANTS["LOT_CHOICEBUILD_ROCKET"]
CONSTANTS["CHOICEBUILD_LOTS"][2] = CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]
CONSTANTS["CHOICEBUILD_LOTS"][3] = CONSTANTS["LOT_CHOICEBUILD_LASER"]
CONSTANTS["CHOICEBUILD_LOTS"][4] = CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]

-- the LOT of the prop produced by each of the choicebuild indices
CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"] = {}
CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"][0] = CONSTANTS["LOT_CHOICEBUILD_LASER"]
CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"][1] = CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]
CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"][2] = CONSTANTS["LOT_CHOICEBUILD_ROCKET"]
CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"][3] = CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]

-- the name to use when saving the concert shell particle effect
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"] = {}
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 		= "flameShellEffect"
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 		= -1
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 			= -1
CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 		= "speakerShellEffect"

-- the name to use when saving the concert hill particle effect
CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"] = {}
CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 			= -1
CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 		= "spotlightHillEffect"
CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 			= "laserHillEffect"
CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 		= -1

-- the name to use when saving the discoball particle effect
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"] = {}
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_ROCKET"]] 	= -1
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"]] 	= -1
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_LASER"]] 		= "laserDiscoballEffect"
CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][CONSTANTS["LOT_CHOICEBUILD_SPEAKER"]] 	= -1


-- 5023 AG - Stage Rocket               OLD 4029 AG – Fog Machine Choice Build
-- 4891 AG - Stage Spot Light           OLD 4030 AG – Spotlight Choice Build	
-- 5024 AG - Stage laser                OLD 4031 AG – Laser Light Choice Build	
-- 4858 AG - Speaker  					OLD 4032 AG – Speaker Choice Build	



local CHOICEBUILDS = nil
local ChoicebuildGroup = "CBGroup"


--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup(self)

	registerWithZoneControlObject(self)
	
	-- let the bonus script(o_ChoicebuildBonus.lua) know the name of our group
	SetGroupName( self, ChoicebuildGroup )
    
	self:SetVar( "bAllAlike", false )

end


--------------------------------------------------------------
-- notification to object
--------------------------------------------------------------
function onNotifyObject(self,msg)

    if msg.name == "ChoicebuildChanged" then
    
		local group = self:GetObjectsInGroup{ group = ChoicebuildGroup, ignoreSpawners = true }.objects
	
	
		-- if we havn't filled in our choicebuilds yet
		if CHOICEBUILDS == nil then
			CHOICEBUILDS = {}
			for i = 1, #group do
				CHOICEBUILDS[group[i]:GetID()] = -1 
			end
			
			-- tell the bonus script to initialize its copy of the array of indices too
			InitializeIndices( self )
		end
	
	
		--set the choicebuild's current state
		CHOICEBUILDS[msg.ObjIDSender:GetID()] = msg.param1
		
		
		-- remember whether they were all alike before this choice
		local bAlikeBefore = DoAllChoicebuildsMatch( self )
		
		
		-- pass it on to the bonus script and have it call RewardBonus if all 4 choicebuilds match
		SetIndex( self, msg.ObjIDSender:GetID(), msg.param1, true )
		
		
		-- check whether they all match now
		-- if they were alike and now aren't, make sure the platforms aren't steps anymore
			-- and that the bonus particles are cancelled
		local bAlikeNow = DoAllChoicebuildsMatch( self )
		if ( bAlikeBefore == true and bAlikeNow == false ) then
			
			self:SetVar( "bAllAlike", false )
			
			GAMEOBJ:GetTimer():CancelTimer("StageResetTime", self)
			SignalMovingPlatforms( self, false )
			
			UpdateParticleEffects( self, -1 )

		end

		
	elseif ( msg.name == "StoreLargePlatform" ) then
		storeObjectByName( self, "LargePlatform", msg.ObjIDSender )
	
	elseif ( msg.name == "StoreSmallPlatform" ) then
		storeObjectByName( self, "SmallPlatform", msg.ObjIDSender )
		
	elseif ( msg.name == "ResetChoiceBuilds" ) then
		resetCBs( self )
		
	elseif msg.name == "ChoicebuildSmashed" then
	
		-- remember that this choicebuild is currently a pile of bricks rather than any of the 4 choices
		CHOICEBUILDS[msg.ObjIDSender:GetID()] = -1
		SetIndex( self, msg.ObjIDSender:GetID(), -1, false )

		-- make sure the platforms aren't steps anymore and that the bonus particles are cancelled
		if ( self:GetVar( "bAllAlike" ) == true ) then
			self:SetVar( "bAllAlike", false )
			GAMEOBJ:GetTimer():CancelTimer("StageResetTime", self)
			SignalMovingPlatforms( self, false )
			UpdateParticleEffects( self, -1 )
		end

    end
end




--------------------------------------------------------------
-- LoadObjectAtPoint
--------------------------------------------------------------
function LoadObjectAtPoint(self, templateID, pathname)
    local pathMsg = LEVEL:GetPathWaypoints (pathname)
	
	if (tostring(type(pathMsg)) == "table") then
		for i, v in pairs(pathMsg) do
            RESMGR:LoadObject {
                objectTemplate = templateID,
                	x = v.pos.x,
                    y = v.pos.y,
                    z = v.pos.z,
                    rw = v.rot.w,
                    rx = v.rot.x,
                    ry = v.rot.y,
                    rz = v.rot.z,
                    owner = self }
		end
    end
end





--------------------------------------------------------------
-- child object loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	-- store who the parent is
	storeParent(self, msg.childID)
  
	UpdateParticleEffects( self, -1 )

end





--------------------------------------------------------------
-- reset the choicebuilds
--------------------------------------------------------------
function resetCBs(self)
	
    -- cancel any bonus particle effects received from having the choicebuilds match
    UpdateParticleEffects( self, -1 )
    

	-- if the choicebuilds are already different, then don't reset them because players are already changing them
	-- we only want to reset them if they are all alike now
	if ( DoAllChoicebuildsMatch( self ) == false ) then
		return
	end


	--if we have the choicebuilds, ask them to reset
	if CHOICEBUILDS ~= nil then
		for k,j in pairs(CHOICEBUILDS) do
			GAMEOBJ:GetObjectByID(k):RequestActivityExit()
		end
    end
    
    local group = self:GetObjectsInGroup{ group = ChoicebuildGroup, ignoreSpawners = true }.objects
	-- reset the choicebuild variables
	CHOICEBUILDS = {}
	for i = 1, #group do
		CHOICEBUILDS[group[i]:GetID()] = -1 
	end

	-- tell the bonus script to reset its indices as well
	InitializeIndices( self )

    
end




--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone(self, msg)
    if (msg.name == "StageResetTime") then
		
		-- tell the stage's moving platforms to move back into their original positions that look like extensions of the stage
		-- they will send a message back once they're in their original positions, so we can reset the choice builds at that time
		SignalMovingPlatforms( self, false )
		
		resetCBs(self)
    end
end




--------------------------------------------------------------
-- tell the two moving platforms to move to the other configuration
-- bSteps should be true if you want them to become steps up to the golden brick
-- and false if you want them to look like an extension of the stage
--------------------------------------------------------------
function SignalMovingPlatforms( self, bSteps )

	local largePlatform = getObjectByName( self, "LargePlatform" )
	local smallPlatform = getObjectByName( self, "SmallPlatform" )
	
	local szSignal = "TurnIntoExtensionOfStage"
	if ( bSteps == true ) then
		szSignal = "TurnIntoSteps"
	end
	
	if( largePlatform ~= nil and largePlatform:Exists() ) then
		largePlatform:NotifyObject{ name = szSignal }
	end
	
	if( smallPlatform ~= nil and smallPlatform:Exists() ) then
		smallPlatform:NotifyObject{ name = szSignal }
	end
	
end




--------------------------------------------------------------
-- returns whether or not the given LOT is one of the choicebuild props
--------------------------------------------------------------
function IsValidChoicebuildLOT( LOT )

    return ( LOT == CONSTANTS["LOT_CHOICEBUILD_ROCKET"] or
			LOT == CONSTANTS["LOT_CHOICEBUILD_SPOTLIGHT"] or
			LOT == CONSTANTS["LOT_CHOICEBUILD_LASER"] or
			LOT == CONSTANTS["LOT_CHOICEBUILD_SPEAKER"] )
    
end




--------------------------------------------------------------
-- update the particle effects due to the current state of the 4 choicebuilds
-- if one of the choicebuild option indices is passed in, play the corresponding effect and cancel any existing ones
-- if -1 is passed in, then cancel any existing effects 
--------------------------------------------------------------
function UpdateParticleEffects( self, index )

	if ( index == -1 ) then
		CancelAllDiscoballEffects( self )
		CancelAllShellEffects( self )
		CancelAllHillEffects( self )
	
	else
		local LOT = CONSTANTS["CHOICEBUILD_LOT_BY_INDEX"][index]
		
		UpdateDiscoballEffects( self, LOT )
		UpdateShellEffects( self, LOT )
		UpdateHillEffects( self, LOT )
		
	end
		
end






--------------------------------------------------------------
-- update the discoball particle effects because all 4 props have the given LOT
--------------------------------------------------------------
function UpdateDiscoballEffects( self, LOT )

	-- get the object that the effects should be attached to
	local discoballGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_DISCOBALL_GROUP"], ignoreSpawners = true }.objects
	
	
	-- get the discoball particle effect info for this LOT
	local desiredEffectNum = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][LOT]
	local desiredEffectSave = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][LOT]
	local desiredEffectName = CONSTANTS["CHOICEBUILD_EFFECT_NAME"][LOT]
	
			
	-- cancel any other effects on the discoball
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local testLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local testEffectNum = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][testLOT]
		
		if ( testEffectNum ~= -1 and testEffectNum ~= desiredEffectNum ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][testLOT]

			for groupIndex = 1, table.maxn ( discoballGroup ) do
				discoballGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
		
	-- start the desired effect on the discoball, if there is one
	if ( desiredEffectNum ~= -1 ) then

		for i = 1, table.maxn ( discoballGroup ) do
			--discoballGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectID = desiredEffectNum, effectType = desiredEffectName }
			discoballGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectType = desiredEffectName }
		end
		
	end
	
end





--------------------------------------------------------------
-- update the concert shell particle effects because all 4 props have the given LOT
--------------------------------------------------------------
function UpdateShellEffects( self, LOT )

	-- get the object that the effects should be attached to
	local shellGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_SHELL_GROUP"], ignoreSpawners = true }.objects
	
	
	-- get the concert shell particle effect info for this LOT
	local desiredEffectNum = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][LOT]
	local desiredEffectSave = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][LOT]
	local desiredEffectName = CONSTANTS["CHOICEBUILD_EFFECT_NAME"][LOT]
	
			
	-- cancel any other effects on the concert stage
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local testLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local testEffectNum = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][testLOT]
		
		if ( testEffectNum ~= -1 and testEffectNum ~= desiredEffectNum ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][testLOT]

			for groupIndex = 1, table.maxn ( shellGroup ) do
				shellGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
		
	-- start the desired effect on the shell, if there is one
	if ( desiredEffectNum ~= -1 ) then

		for i = 1, table.maxn ( shellGroup ) do
			--shellGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectID = desiredEffectNum, effectType = desiredEffectName }
			shellGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectType = desiredEffectName }
		end
		
	end
	
end





--------------------------------------------------------------
-- update the concert hill particle effects because all 4 props have the given LOT
--------------------------------------------------------------
function UpdateHillEffects( self, LOT )

	-- get the object that the effects should be attached to
	local hillGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_HILL_GROUP"], ignoreSpawners = true }.objects
	
	
	-- get the concert shell particle effect info for this LOT
	local desiredEffectNum = CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][LOT]
	local desiredEffectSave = CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][LOT]
	local desiredEffectName = CONSTANTS["CHOICEBUILD_EFFECT_NAME"][LOT]
	
			
	-- cancel any other effects on the concert hill
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local testLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local testEffectNum = CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][testLOT]
		
		if ( testEffectNum ~= -1 and testEffectNum ~= desiredEffectNum ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][testLOT]

			for groupIndex = 1, table.maxn ( hillGroup ) do
				hillGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
		
	-- start the desired effect on the shell, if there is one
	if ( desiredEffectNum ~= -1 ) then

		for i = 1, table.maxn ( hillGroup ) do
			--shellGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectID = desiredEffectNum, effectType = desiredEffectName }
			hillGroup[i]:PlayFXEffect{ name = desiredEffectSave, effectType = desiredEffectName }
		end
		
	end
	
end





--------------------------------------------------------------
-- cancel any effects on the discoball
--------------------------------------------------------------
function CancelAllDiscoballEffects( self )

	-- get the object that any existing effects are attached to
	local discoballGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_DISCOBALL_GROUP"], ignoreSpawners = true }.objects
	
			
	-- cancel any effects on the discoball
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local nextLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local nextEffectNum = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_NUM"][nextLOT]
		
		if ( nextEffectNum ~= -1 ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_DISCOBALL_EFFECT_SAVE"][nextLOT]

			for groupIndex = 1, table.maxn ( discoballGroup ) do
				discoballGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
end





--------------------------------------------------------------
-- cancel any effects on the concert stage
--------------------------------------------------------------
function CancelAllShellEffects( self )

	-- get the object that any existing effects are attached to
	local shellGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_SHELL_GROUP"], ignoreSpawners = true }.objects
	
			
	-- cancel any effects on the concert shell
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local nextLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local nextEffectNum = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_NUM"][nextLOT]
		
		if ( nextEffectNum ~= -1 ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_SHELL_EFFECT_SAVE"][nextLOT]

			for groupIndex = 1, table.maxn ( shellGroup ) do
				shellGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
end





--------------------------------------------------------------
-- cancel any effects on the concert hill
--------------------------------------------------------------
function CancelAllHillEffects( self )

	-- get the object that any existing effects are attached to
	local hillGroup= self:GetObjectsInGroup{ group = CONSTANTS["CHOICEBUILD_HILL_GROUP"], ignoreSpawners = true }.objects
	
			
	-- cancel any effects on the concert shell
	for LOTindex = 1, #CONSTANTS["CHOICEBUILD_LOTS"] do
	
		local nextLOT = CONSTANTS["CHOICEBUILD_LOTS"][LOTindex]
		local nextEffectNum = CONSTANTS["CHOICEBUILD_HILL_EFFECT_NUM"][nextLOT]
		
		if ( nextEffectNum ~= -1 ) then
		
			local cancelEffectName = CONSTANTS["CHOICEBUILD_HILL_EFFECT_SAVE"][nextLOT]

			for groupIndex = 1, table.maxn ( hillGroup ) do
				hillGroup[groupIndex]:StopFXEffect{ name = cancelEffectName }
			end
		
		end
		
	end
	
end




--------------------------------------------------------------
-- called by the bonus script when we just set the index on one of the choicebuilds
-- and now they all match
--------------------------------------------------------------
function RewardBonus( self, index )

	self:SetVar( "bAllAlike", true )

		
	-- start a timer to reset the choicebuilds

	GAMEOBJ:GetTimer():AddTimerWithCancel( 30.0 , "StageResetTime", self )
	
	-- tell the stage's moving platforms to move so that they become steps up to the golden brick
	SignalMovingPlatforms( self, true )

	UpdateParticleEffects( self, index )
			
end







