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

  
    Set['OverRideHealth']   = false   -- Bool Health Overide
    Set['Health']           = 7       -- Amount of health

    Set['OverRideImag']     = false   -- Bool Imagination Overide
    Set['Imagination']      = nil     -- Amout of Imagination

    Set['OverRideImmunity'] = false   -- Bool Immunity Overide
    Set['Immunity']         = false   -- Bool
    
    Set['OverRideName']     = false
    Set['Name']             = "Freak Darkling" 

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
    Set['aggroRadius']      = 0     -- Aggro Radius
    Set['conductRadius']    = 0     -- Conduct Radius
    Set['tetherRadius']     = 0     -- Tether  Radius
    Set['tetherSpeed']      = 0      -- Tether Speed
    Set['wanderRadius']     = 0      -- Wander Radius
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
    Set['WayPointSet']       = ""     -- ["name of way point set"]
    Set['WayPointDelay']     =  nil     -- ["ends"],["all"],[nil]
    Set['WayPointDMin']      = 0            -- Min Waypoint Delay
    Set['WayPointDMax']      = 0            -- Max Waypoint Delay
    Set['WayPointSpeed']     = 1.4         -- Waypoint Speed (Travle speed) 
    -- Wander Settings ----------------------------------------------------
    Set['WanderEmote']       = false        -- Enable bool
    Set['WanderChance']      = 100          -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
    -- effect 1
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
    -- effect 2
    -- If emote 1 Weight is 20 emote 2 Weight will be 80....ect 
    Set['WEmote_2']          = false         -- Enalble 2nd Emote bool
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
    Set['AggroNPC']        = false
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

    Set['Con_1_Action']     = false        -- Active true/false
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

    Set['Conduct_2_Active']  = false       -- Conduct 1 Active true/false
--------------------------------------------------------------------------------------

-- Action 

    Set['Con_2_Action']     = false        -- Active true/false
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
    
 --[[ 
--/////////////////////////////////////////////////////////////////////////

         ____    _____   _____    ____   _          _      ____    ____  
        |  _ \  | ____| |_   _|  / ___| | |        / \    / ___|  / ___| 
        | |_) | |  _|     | |   | |     | |       / _ \   \___ \  \___ \ 
        |  __/  | |___    | |   | |___  | |___   / ___ \   ___) |  ___) |
        |_|     |_____|   |_|    \____| |_____| /_/   \_\ |____/  |____/         
         
--]]

    
    Set['Pet_Active']           = false -- Active [true/false] 
	
    Set['Pet_ParentMovement']   = false -- Lock parent in a frozen state Cant't attack or move 
    Set['Pet_DeathLinked']      = true  -- When Parent dies pets will despawn
    Set['Pet_AggroLinked']      = true  -- If Parent is attacked the childern will assist
    Set['Pet_RespawnPets']      = true
    Set['Pet_SpawnID']          = 1876  -- Spawner ID [1876 = Grumpy] 
    Set['Pet_Count']            = 2     -- Number of pets the given parent can have
    Set['Pet_Respawn']          = 10    -- Pet Respawn time 
    Set['Pet_DisMin']           = 10    -- Radius Min Distance
    Set['Pet_DisMax']           = 20    -- Radius Max Distance
    Set['Pet_Follow']           = false -- Pet will follow the Parent
    Set['Pet_FollowDis']        = 5     -- Distance to follow 

    --  Not working right now --
    Set['Pet_HealthLink']       = false
    Set['Pet_UseEffect']        = false
    Set['Pet_SpawnEffectID']    = 4
    Set['Pet_SpawnType']        = "name"

    Set['Pet_Child']            = false
    Set['Pet_ChildMovement']    = "follow"  
	
------ Due not change --------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
-------------------------------------------------------------------------------

end

function onGetActivityPoints(self, msg)
	msg.points = 100
	return msg
end






















