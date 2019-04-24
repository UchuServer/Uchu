require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
function onStartup(self) 
Set = {}
	
--[[
///////////////////////////////////////////////////////////////////////////
         ____    _  __  ___   _       _       ____  
        / ___|  | |/ / |_ _| | |     | |     / ___| 
        \___ \  | ' /   | |  | |     | |     \___ \ 
         ___) | | . \   | |  | |___  | |___   ___) |
        |____/  |_|\_\ |___| |_____| |_____| |____/                                                                                     
--]]

    --Set['skillID']          = 9  
  
    Set['OverRideHealth']   = false   -- Bool Health Overide
    Set['Health']           = 500     -- Amount of health

    Set['OverRideImag']     = false   -- Bool Imagination Overide
    Set['Imagination']      = nil     -- Amout of Imagination

    Set['OverRideImmunity'] = false   -- Bool Immunity Overide
    Set['Immunity']         = false   -- Bool
    
    Set['OverRideName']     = true
    Set['Name']             = "Dragon Boss" 

    Set['EmoteReact']       = false
    Set['Emote_Delay']      = 2
    Set['React_Set']        = "test"
--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/         
--]]
    Set['aggroRadius']      = 40     -- Aggro Radius
    Set['conductRadius']    = 0     -- Conduct Radius
    Set['tetherRadius']     = 0     -- Tether  Radius
    Set['tetherSpeed']      = 8      -- Tether Speed
    Set['wanderRadius']     = 8      -- Wander Radius
    --- FOV Radius -- 
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 180 
    -- Conduct
    Set['UseConductFOV']    = false
    Set['conductFOV']       = 180 
 --[[
///////////////////////////////////////////////////////////////////////////
         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
--]]

    --**********************************************************************
    Set['MovementType']     = "Guard" --["Patroler"],["Guard"],["Wander"]
    --**********************************************************************
    -- Patrol Settings ---------------------------------------------------- 
    Set['WayPointType']      = "linear"     -- ["loop"],["linear"],["once"]
    Set['WayPointSet']       = "sq_walk"     -- ["name of way point set"]
    Set['WayPointDelay']     =  nil     -- ["ends"],["all"],[nil]
    Set['WayPointDMin']      = 2            -- Min Waypoint Delay
    Set['WayPointDMax']      = 2            -- Max Waypoint Delay
    Set['WayPointSpeed']     = 1            -- Waypoint Speed (Travle speed) 
    -- Wander Settings ----------------------------------------------------
    Set['WanderEmote']       = false        -- Enable bool
    Set['WanderChance']      = 100          -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    -- effect 1
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
    -- effect 2
    -- If emote 1 Weight is 20 emote 2 Weight will be 80....ect 
    Set['WEmote_2']          = true         -- Enalble 2nd Emote bool
    Set['WEmoteType_2']      = "breakdance" -- Animation Type
--[[
/////////////////////////////////////////////////////////////////////////
            _       ____    ____   ____     ___  
           / \     / ___|  / ___| |  _ \   / _ \ 
          / _ \   | |  _  | |  _  | |_) | | | | |
         / ___ \  | |_| | | |_| | |  _ <  | |_| |
        /_/   \_\  \____|  \____| |_| \_\  \___/       
        
--]] 
    Set['Aggression']     = "Passive"  -- [Aggressive]--[Neutral]--[Passive]-[PassiveAggres]-
    Set['AggroNPC']       = false
    Set['AggroDist']      = 2              -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 1              -- Multiplier of the NPC's base speed to approach while attacking      

    -- Aggro Emote
    Set['AggroEmote']      = false     --Plays Emote on Aggro 
    Set['AggroE_Type']     = ""        -- String Name of Emote
    Set['AggroE_Delay']    = 1         -- Animation Delay Time
    
    -- NPC Hated List
    Set['NPCHated_1']      = nil       --Faction of a NPC 
    Set['NPCHated_2']      = nil       --Faction of a NPC
    Set['NPCHated_3']      = nil       --Faction of a NPC
    Set['NPCHated_4']      = nil       --Faction of a NPC
 --[[
///////////////////////////////////////////////////////////////////////////
         _____   _____      _      ____  
        |  ___| | ____|    / \    |  _ \ 
        | |_    |  _|     / _ \   | |_) |
        |  _|   | |___   / ___ \  |  _ < 
        |_|     |_____| /_/   \_\ |_| \_\     

    Fear uses the ConductRadius you may NOT enable a conduct behavior when using Fear 
             
--]]               

    -- Fear Settings
    Set['FearPlayer']      = false      -- Fear the Player Bool
    Set['FearNPC']         = false      -- Fear Other NPC Bool
    Set['FearCombat']      = false      -- Fear when other NPC/Player are in combat
    Set['FearHP']          = false      -- Fear base on remaining HP

     
    Set['FearHPamount']    = 1          -- The amout of HP need to fear

    -- ['FearTime'] the amout of time to flee,, 
    -- If ['FearHP'] = true NPC will retrun and attack after time has expired. 
    Set['FearChance']      = 100 
    Set['FearType']        = "Flee"    -- ['Flee'],['FleeEmote'], ['EmoteFlee']
    Set['FearDistance']    = 60        -- Flee Distance 
    Set['FearTime']        = 1         -- Time before retruning to the orginal pos. 
    Set['FearSpeed']       = 5         -- NPC walk speed
    Set['FearFace']        = true      -- NPC will face target when it has reached its flee pos. 
    Set['FearFOV']         = 220       -- [int/nil] FOV setting NPC must see you before Fleeing [Note: conduct FOV must be set to -true- ] 
                                       -- The conduct Radius will be nilled out if FOV is used.    

    -- optional: Use for Flee Taunt -- 
    Set['FearEmoteType']   = "breakdance" -- Animation Type
        
    -- Fear NPC list
    Set['FearNPC_1']       = nil       --Faction of a NPC
    Set['FearNPC_2']       = nil       --Faction of a NPC
    Set['FearNPC_3']       = nil       --Faction of a NPC
    Set['FearNPC_4']       = nil       --Faction of a NPC


--[[ 
--/////////////////////////////////////////////////////////////////////////

         _   _   _____   _       ____    _____   ____  
        | | | | | ____| | |     |  _ \  | ____| |  _ \ 
        | |_| | |  _|   | |     | |_) | |  _|   | |_) |
        |  _  | | |___  | |___  |  __/  | |___  |  _ < 
        |_| |_| |_____| |_____| |_|     |_____| |_| \_\  
        
        
--]]

    Set['Helper']           = false       -- Bool
    Set['HelperType']       = "string"    -- ["Aggro"],["Health"],["investigate"]
    Set['HelperDis']        = 20          -- The Distance to call for help 
    Set['HelperFaction']    = 4           -- Faciton to help

--[[ 
--/////////////////////////////////////////////////////////////////////////

          ____    ___    _   _   ____    _   _    ____   _____      _ 
         / ___|  / _ \  | \ | | |  _ \  | | | |  / ___| |_   _|    / |
        | |     | | | | |  \| | | | | | | | | | | |       | |      | |
        | |___  | |_| | | |\  | | |_| | | |_| | | |___    | |      | |
         \____|  \___/  |_| \_| |____/   \___/   \____|   |_|      |_|   
         
         
--]]

    Set['Conduct_1_Active']  = false       -- Conduct 1 Active true/false
--------------------------------------------------------------------------------------

-- Action 

    Set['Con_1_Action']     = true        -- Active true/false
    Set['Con_1_AFaction']   = 1           -- Faction of the NPC/Player
    Set['Con_1_AChance']    = 100         -- Chance to Play
    Set['Con_1_Order']      = "before"    -- When Action is played before/after
    Set['Con_1_Type']       = "Ninja"     -- String name of type ['Ninja'],['face']
    Set['Con_1_Distance']   = 20          -- Option Distance
    Set['Con_1_ATarget']    = true        -- Target NPC/Player before playing Action
    Set['Con_1_DelayMin']   = 1           -- Min Delay
    Set['Con_1_DelayMax']   = 2           -- Max Delay

-- Emote 

    Set['Con_1_Emote']      = true        -- Active true/false
    Set['Con_1_Weight']     = 100         -- Chance to play 
    Set['Con_1_EffectType'] = "salute"    -- The Effect Name
    Set['Con_1_ETarget']    = true        -- Turns the NPC to the Target Before Emoting 
    Set['Con_1_EFaction']   = 100         -- Faction of the NPC/Player

--[[ 
--/////////////////////////////////////////////////////////////////////////

          ____    ___    _   _   ____    _   _    ____   _____      ____  
         / ___|  / _ \  | \ | | |  _ \  | | | |  / ___| |_   _|    |___ \ 
        | |     | | | | |  \| | | | | | | | | | | |       | |        __) |
        | |___  | |_| | | |\  | | |_| | | |_| | | |___    | |       / __/ 
         \____|  \___/  |_| \_| |____/   \___/   \____|   |_|      |_____|
         
         
--]]

    Set['Conduct_2_Active']  = false       -- Conduct 2 Active true/false
--------------------------------------------------------------------------------------

-- Action 

    Set['Con_2_Action']     = true        -- Active true/false
    Set['Con_2_AFaction']   = 1           -- Faction of the NPC/Player
    Set['Con_2_AChance']    = 100         -- Chance to Play
    Set['Con_2_Order']      = "before"    -- When Action is played before/after
    Set['Con_2_Type']       = "Ninja"     -- String name of type ['Ninja'],['face']
    Set['Con_2_Distance']   = 20          -- Option Distance
    Set['Con_2_ATarget']    = true        -- Target NPC/Player before playing Action
    Set['Con_2_DelayMin']   = 1           -- Min Delay
    Set['Con_2_DelayMax']   = 2           -- Max Delay

-- Emote 

    Set['Con_2_Emote']      = true        -- Active true/false
    Set['Con_2_Weight']     = 100         -- Chance to play 
    Set['Con_2_EffectType'] = "salute"    -- The Effect Name
    Set['Con_2_ETarget']    = true        -- Turns the NPC to the Target Before Emoting 
    Set['Con_2_EFaction']   = 100         -- Faction of the NPC/Player
    
    
--/////////////////////////////////////////////////////////////////////////
    
------ Due not change --------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
-------------------------------------------------------------------------------

	-- dragon boss special code
	onDragonBossInit(self)
    
end

--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS = {}

-- Default Skill ID for the projectile
CONSTANTS["IMPACT_SKILLID"] = 71

-- template for the fireball projectile
CONSTANTS["PROJECTILE_TEMPLATEID"] = 2716

-- rough height and length of the dragon boss (to spawn fireballs from)
CONSTANTS["DRAGON_HEIGHT"] = 20.0
CONSTANTS["DRAGON_LENGTH"] = 95.0

-- rough height of the minifig's torso (so fireballs can target them better)
CONSTANTS["MINIFIG_HEIGHT"] = 1.5

-- how fast the fireballs move in meters per sec
CONSTANTS["PROJECTILE_SPEED"] = 85.0

-- the distance at which the dragon will start attacking opponents
CONSTANTS["DRAGON_AGGRO_RADIUS"] = 500.0

-- how often the dragon's shoots his fire balls in seconds (range of time)
CONSTANTS["ATTACK_DELAY_MIN"] = 1.5
CONSTANTS["ATTACK_DELAY_MAX"] = 4.5

-- amount of time between when dragon starts attack animation and fireball actually spawns
CONSTANTS["FIREBALL_ANIM_DELAY"] = 0.05

-- an area around the player at which the dragon actually attacks (never dead-on)
CONSTANTS["FIREBALL_TARGET_OFFSET"] = 7.0

-- the number of targets that the dragon will keep track of at once
CONSTANTS["MAX_DRAGON_TARGETS"] = 10


--------------------------------------------------------------
-- Special Init Code
--------------------------------------------------------------
function onDragonBossInit(self)

	print("Creating Dragon")
	
	for u = 1,CONSTANTS["MAX_DRAGON_TARGETS"] do
		self:SetVar("Attacking"..u, false)
		self:SetVar("TargetOpponent"..u, "")
	end
    
    self:SetProximityRadius { radius = CONSTANTS["DRAGON_AGGRO_RADIUS"], name = "SeekOpponents" }
    
    -- kick off the initial search (if he has no targets, he'll just search again)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "AttackOpponents", self )
    
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)

	--print("Timer Hit")
    
    if msg.name == "AttackOpponents" then 
		
		print("Getting Ready to Attack")
		
		self:PlayAnimation{animationID = "attack"}
		
	--	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["FIREBALL_ANIM_DELAY"], "ShootFireball", self )
	--	
	--elseif msg.name == "ShootFireball" then
    --
	--	print("Shooting Fireball!")
	
		local targetID = getRandomTarget(self)
		if targetID ~= "" then
		
			finalID = "|" .. targetID
			self:SetVar("ProjectileTargetID", finalID) -- store the ID so we know where to aim the projectile when it's ready
	    
			local spawnPos = self:GetPosition().pos 
			local spawnFwd = self:GetPlayerForward().fwd
			spawnFwd.x = spawnFwd.x * CONSTANTS["DRAGON_LENGTH"]
			spawnFwd.z = spawnFwd.z * CONSTANTS["DRAGON_LENGTH"]
	    
			-- spawn the projectile
			RESMGR:LoadObject {
				objectTemplate = CONSTANTS["PROJECTILE_TEMPLATEID"],
				x = spawnPos.x+spawnFwd.x,
				y = spawnPos.y+CONSTANTS["DRAGON_HEIGHT"],
				z = spawnPos.z+spawnFwd.z,
				rw = 1,
				owner = self
			}
			
		end
		
		local delay_time = math.random(CONSTANTS["ATTACK_DELAY_MIN"],CONSTANTS["ATTACK_DELAY_MAX"])
		GAMEOBJ:GetTimer():AddTimerWithCancel( delay_time, "AttackOpponents", self )
    
	end
    
end



--------------------------------------------------------------
-- Called after loading a projectile
--------------------------------------------------------------
function onChildLoaded(self, msg)

	--print("Child Loaded")
		
	-- if we loaded a projectile, fire it
	if msg.templateID == CONSTANTS["PROJECTILE_TEMPLATEID"] then 

		-- print("Getting Target...")
	
	    -- store who the parent is
	    storeParent(self, msg.childID)
		local projObj = msg.childID

		-- get the skill for the projectile		
		--skill = self:GetVar("ImpactSkillID")
		skill = CONSTANTS["IMPACT_SKILLID"]
		
		-- get the target that we're firing at
		local enemyID = self:GetVar("ProjectileTargetID")

		if (projObj) and (skill) and (enemyID ~= "") then
		
			print("Fireball Created")
		
			-- store values in the projectile
			projObj:SetVar("ImpactSkillID", skill)
			projObj:SetVar("My_Faction", self:GetFaction().faction)
			--projObj:SetFaction{faction = self:GetFaction().faction}

			-- position of ourselves and the target
			local enemyObj = GAMEOBJ:GetObjectByID(enemyID)
			if (enemyObj ~= nil) then
			
				-- give it a small amount of variance so it's not perfectly spot-on
				local enemyPos = Vector.new(getRandomPos(self,enemyObj:GetPosition().pos,CONSTANTS["FIREBALL_TARGET_OFFSET"]))				
				enemyPos.y = enemyPos.y + CONSTANTS["MINIFIG_HEIGHT"]
				local localPos = Vector.new(projObj:GetPosition().pos)
				local dir =  enemyPos - localPos
				local firstVec = dir:normalize() * CONSTANTS["PROJECTILE_SPEED"] 

				-- store the velocity and force it to be heading in a downward direction (since right now projectiles only die when they hit the ground)
				-- TODO -> get projectiles to also have a time and/or distance limit on them so they don't go flying off forever, then get rid of this
				local vec = { x = firstVec.x, y = firstVec.y, z = firstVec.z }
				if (vec.y > -0.5) then
					vec.y = -0.5
				end

				-- set projectile params
				projObj:SetProjectileParams{initVel = vec, iProjectileType = 2, fLifeTime = 10.0, owner = self}
				
			end
			
		end
		
	end
	
end



--------------------------------------------------------------
-- When enemies enter the proximity radius
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	print("Proximity Update!")

	-- if this was a proximity update for seeking an opponent and the object is of the 'player faction'...	
	if ( ( msg.name == "SeekOpponents" ) and ( msg.objId:GetFaction().faction == 1 ) ) then        ------- and ( msg.objType == "Enemies" )
	
		print("Player Proximity Change!")
		
		if ( msg.status == "ENTER" ) then
		
			addTarget( self, msg.objId:GetID() )
		
		else
		
			removeTarget( self, msg.objId:GetID() )
			
		end
		
	end
	
end




--------------------------------------------------------------
-- Stores an object ID into an 'array' of potential targets
--------------------------------------------------------------
function addTarget(self, objId)

	local alreadyTargeted = false
	local emptySlot = 0

	for u = 1,CONSTANTS["MAX_DRAGON_TARGETS"] do
		if ( self:GetVar("Attacking"..u) ~= true ) then
		
			if ( emptySlot == 0 ) then
				emptySlot = u
			end
			
			if ( self:GetVar("TargetOpponent"..u) == objId ) then
				alreadyTargeted = true
			end
			
		end
	end
	
	if ( alreadyTargeted == false and emptySlot ~= 0 ) then
	
		--print("Enemy Within Radius")
		
		self:SetVar("Attacking"..emptySlot, true)
		finalID = "|" .. objId
		self:SetVar("TargetOpponent"..emptySlot, finalID)
			
	end
end


--------------------------------------------------------------
-- Removes an object ID from the 'array' of potential targets
--------------------------------------------------------------
function removeTarget(self, objId)

	for u = 1,CONSTANTS["MAX_DRAGON_TARGETS"] do
		if ( self:GetVar("Attacking"..u) == true and self:GetVar("TargetOpponent"..u) == objId ) then
			
			--print("Enemy Leaving Radius")
			
			self:SetVar("Attacking"..u, false)
			self:SetVar("TargetOpponent"..u, "")
		
		end
	end
	
end


--------------------------------------------------------------
-- Returns an object ID at random from the 'array' of potential targets
--------------------------------------------------------------
function getRandomTarget(self)

	local totalTargets = 0

	-- count up how many targets to random between
	for u = 1,CONSTANTS["MAX_DRAGON_TARGETS"] do
		if ( self:GetVar("Attacking"..u) == true ) then
			totalTargets = totalTargets + 1
		end
	end
	
	if ( totalTargets > 0 ) then

		local randomTarget = math.random(1,totalTargets)
		
		--print( "random target " .. randomTarget )

		-- now run through that many available targets for the actual one
		for u = 1,CONSTANTS["MAX_DRAGON_TARGETS"] do
			if ( self:GetVar("Attacking"..u) == true ) then
				randomTarget = randomTarget - 1
				if ( randomTarget == 0 ) then
					return self:GetVar("TargetOpponent"..u)
				end
			end
		end
		
	end
	
	return ""
end














--------------------------------------------------------------
--------------------------------------------------------------
--------------------------------------------------------------
--------------------------------------------------------------
---- OLD CODE BEING SAVED 'JUST IN CASE' BELOW THIS POINT ----
--------------------------------------------------------------
--------------------------------------------------------------
--------------------------------------------------------------
--------------------------------------------------------------

--	-- if this was a proximity update for seeking an opponent and the object is of the 'player faction'...	
--	if ( ( msg.name == "SeekOpponents" ) and ( msg.objId:GetFaction().faction == 1 ) ) then        ------- and ( msg.objType == "Enemies" )
--	
--		print("Player Proximity Change!")
--		
--		if ( msg.status == "ENTER" ) then
--		
--			if ( self:GetVar("Attacking") ~= true ) then
--			
--				print("Enemy Within Radius")
--				self:SetVar("Attacking", true)
--				finalID = "|" .. msg.objId:GetID()
--				self:SetVar("TargetOpponent", finalID)
--				GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "AttackOpponents", self )
--				
--			end
--			
--			addTarget( self, msg.objId:GetID() )
--		
--		else
--		
--			if ( self:GetVar("Attacking") == true and self:GetVar("TargetOpponent") == msg.objId:GetID() ) then
--			
--				print("Enemy Leaving Radius")
--				self:SetVar("Attacking", false)
--				self:SetVar("TargetOpponent", "")
--				GAMEOBJ:GetTimer():CancelAllTimers( self )
--                
--			end
--
--			removeTarget( self, msg.objId:GetID() )
--			
--		end
--		
--	end
--	
--end
