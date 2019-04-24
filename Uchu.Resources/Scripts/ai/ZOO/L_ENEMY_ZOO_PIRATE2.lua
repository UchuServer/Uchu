require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
function onStartup(self) 
Set = {}
    
    
   Set['FollowActive'] = true
  
--[[
///////////////////////////////////////////////////////////////////////////

         ____    _  __  ___   _       _       ____  
        / ___|  | |/ / |_ _| | |     | |     / ___| 
        \___ \  | ' /   | |  | |     | |     \___ \ 
         ___) | | . \   | |  | |___  | |___   ___) |
        |____/  |_|\_\ |___| |_____| |_____| |____/ 
                                                                                       
--]]

    Set['skillID']          = 11    

    Set['OverRideHealth']   = false 
    Set['Health']           = 3
   
    Set['OverRideImag']     = false
    Set['Imagination']      = 10

    Set['OverRideImmunity'] = false
    Set['Immunity']         = false

    Set['OverRideName']     = true
    Set['Name']             = "NINJA"  

    Set['EmoteReact']       = true
    Set['Emote_EffectID']   = 4 
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
    
    -- Aggro
    Set['OverRideAggro']    = true  
    Set['aggroRadius']      = 10
    -- Aggro FOV
    Set['UseAggroFOV']      = false
    Set['aggroFOVRadius']    = 20
    Set['aggroFOV']         = 180 
    -- Conduct
    Set['OverRideConduct']  = true 
    Set['conductRadius']    = 20

    -- Conduct FOV 
    Set['UseConductFOV']    = true
    Set['conductFOVRadius'] = 15
    Set['conductFOV']       = 180 
    -- Tether
    Set['OverRideTether']   = true 
    Set['tetherRadius']     = 45
    -- Wander
    Set['OverRideWander']   = true 
    Set['wanderRadius']     = 8
    
    
 --[[
///////////////////////////////////////////////////////////////////////////

         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
        
--]]

    --**********************************************************************
    Set['MovementType']     = "Guard" --["Patroler"],["Guard"],["Wander"],[Frozen]
    --**********************************************************************

    -- Patrol Settings ---------------------------------------------------- 

    Set['WayPointType']      = "linear"     -- ["loop"],["linear"],["once"]
    Set['WayPointSet']       = "small"     -- ["name of way point set"]
    Set['WayPointDelay']     = nil          -- ["ends"],["all"],[nil]
    Set['WayPointDMin']      = 0
    Set['WayPointDMax']      = 0
    Set['WayPointSpeed']     = 1
    
    -- Wander Settings ----------------------------------------------------

    Set['WanderEmote']       = false
    Set['WanderChance']      = 100
    Set['WanderDelayMin']    = 5
    Set['WanderDelayMax']    = 5

    -- effect 1
    Set['WEmote_1']          = 30         -- Weight 
    Set['WEmoteType_1']      = "salute"
    Set['WEmoteEffe_1']      = 4
    -- effect 2
    -- If emote 1 Weight is 20 emote 2 Weight will be 80....ect 
    Set['WEmote_2']          = true
    Set['WEmoteType_2']      = "breakdance"
    Set['WEmoteEffe_2']      = 4

--[[
/////////////////////////////////////////////////////////////////////////
            _       ____    ____   ____     ___  
           / \     / ___|  / ___| |  _ \   / _ \ 
          / _ \   | |  _  | |  _  | |_) | | | | |
         / ___ \  | |_| | | |_| | |  _ <  | |_| |
        /_/   \_\  \____|  \____| |_| \_\  \___/       
        
--]] 
  
    Set['Aggression']     = "Neutral"  -- [Aggressive]--[Neutral]["Friendly"]
    Set['AggroNPC']        = false

    -- Aggro Emote
    Set['AggroEmote']      = false     --Plays Emote on Aggro 
    Set['AggroE_Type']     = ""        -- String Name of Emote
    Set['AggroE_ID']       = 3         -- effectID of Emmote
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
    Set['FearPlayer']      = false
    Set['FearNPC']         = false 
    Set['FearCombat']      = false
    Set['FearHP']          = false 

     
    Set['FearHPamount']    = 1

    -- ['FearTime'] the amout of time to flee,, 
    -- If ['FearHP'] = true NPC will retrun and attack after time has expired. 
    Set['FearChance']      = 100 
    Set['FearType']        = "Flee"-- ['Flee'],['FleeEmote'], ['EmoteFlee']
    Set['FearDistance']    = 20         -- Flee Distance 
    Set['FearTime']        = 8         -- Time before retruning to the orginal pos. 
    Set['FearSpeed']       = 3         -- NPC walk speed
    Set['FearFace']        = true      -- NPC will face target when it has reached its flee pos. 
    Set['FearFOV']         = nil       -- [int/nil] FOV setting NPC must see you before Fleeing [Note: conduct FOV must be set to -true- ] 
                                       -- The conduct Radius will be nilled out if FOV is used.    

    -- optional: Use for Flee Taunt -- 
    Set['FearEmoteType']   = "salute"
    Set['FearEffectID']    = 4
    
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

    Set['Helper']           = false       -- 
    Set['HelperType']       = "string"    -- ["Aggro"],["Health"],["investigate"]
    Set['HelperDis']        = 20          -- The Distance to call for help 
    Set['HelperFaction']    = 4

--[[ 
--/////////////////////////////////////////////////////////////////////////

          ____    ___    _   _   ____    _   _    ____   _____      _ 
         / ___|  / _ \  | \ | | |  _ \  | | | |  / ___| |_   _|    / |
        | |     | | | | |  \| | | | | | | | | | | |       | |      | |
        | |___  | |_| | | |\  | | |_| | | |_| | | |___    | |      | |
         \____|  \___/  |_| \_| |____/   \___/   \____|   |_|      |_|   
         
         
--]]

    Set['Conduct_MainWeight']= 100 
    
    Set['Conduct_1_Active']  = false      -- Conduct 1 Active true/false
    Set['Con_1_AFaction']    = 1          -- Faction of the NPC/Player
    Set['Conduct_CoolDown']  = 5          -- Cool down is start after the conduct has completed. 
--------------------------------------------------------------------------------------
        --  Actions

        --  ["follow"]  : Follow the Set faction 
        --  ["face"]    : Face the set faction 
        --  ["sneakto"] : Check to see if target can see him then follow
        --  ["flee"]    : Use Fear setting for flee ( Due not use Emote ) 
        --  ["goto"]    : Goes to set Faction or [x,y,z]
        --  ["teleport"]: teleport to [x,y,z] pos 

-- Action 

    Set['Con_1_Action']     = false        -- Active true/false
    Set['Con_1_AChance']    = 100          -- Chance to Play
    Set['Con_1_Order']      = "after"     -- When Emote is played ['before']/['after']/['both']. 
    Set['Con_1_Type']       = "flee"     -- String name of type ['follow'],['face']
    Set['Con_1_Distance']   = 3           -- Option Distance [ follow = distance to npc ]
    Set['Con_1_ATarget']    = true        -- Target NPC/Player before playing Action ( Targets and Truns the NPC ) 
    Set['Con_1_Delay']      = 2           -- Delay after the Acton is completed 
    Set['Con_1_Speed']      = 5           -- move speed 
    
    -- goto set Pos [ Optional ]
    Set['Con_1_Use_xyz']    = false
    Set['Con_1_gotoX']      = 0           -- x
    Set['Con_1_gotoY']      = 0           -- y
    Set['Con_1_gotoZ']      = 0           -- z

-- Emote 

    Set['Con_1_Emote']      = true        -- Active true/false
    Set['Con_1_EffectID']   = 4           -- Effect ID 
    Set['Con_1_EffectType'] = "salute"    -- The Effect Name
    Set['Con_1_ETarget']    = true        -- Turns the NPC to the Target Before Emoting 
    Set['Con_1_EDelay']     = 0.5          -- Animation Delay
    Set['Con_1_ESkill']      = true
    Set['Con_1_ESkillID']    = 11
--[[ 
--/////////////////////////////////////////////////////////////////////////

          ____    ___    _   _   ____    _   _    ____   _____      ____  
         / ___|  / _ \  | \ | | |  _ \  | | | |  / ___| |_   _|    |___ \ 
        | |     | | | | |  \| | | | | | | | | | | |       | |        __) |
        | |___  | |_| | | |\  | | |_| | | |_| | | |___    | |       / __/ 
         \____|  \___/  |_| \_| |____/   \___/   \____|   |_|      |_____|
         
         
--]]

    Set['Conduct_2_Active']  = false       -- Conduct 1 Active true/false
    Set['Con_2_AFaction']    = 3           -- Faction of the NPC/Player
--------------------------------------------------------------------------------------
        --  Actions

        --  ["follow"]  : Follow the Set faction 
        --  ["face"]    : Face the set faction 
        --  ["sneakto"] : Check to see if target can see him then follow
        --  ["flee"]    : Use Fear setting for flee ( Due not use Emote ) 
        --  ["goto"]    : Goes to set Faction or [x,y,z]
        --  ["teleport"]: teleport to [x,y,z] pos 
        
-- Action 

    Set['Con_2_Action']     = true        -- Active true/false
    Set['Con_2_AChance']    = 100         -- Chance to Play
    Set['Con_2_Order']      = "after"    -- When Action is played before/after
    Set['Con_2_Type']       = "sneakto"    -- String name of type ['follow'],['face']
    Set['Con_2_Distance']   = 4           -- Option Distance [ follow = distance to npc ]
    Set['Con_2_ATarget']    = true        -- Target NPC/Player before playing Action
    Set['Con_2_DelayMin']   = 1           -- Min Delay
    Set['Con_2_Speed']      = 2         -- move speed 
    
    -- goto set Pos [ Optional ]
    Set['Con_2_Use_xyz']    = false
    Set['Con_2_gotoX']      = 0           -- x
    Set['Con_2_gotoY']      = 0           -- y
    Set['Con_2_gotoZ']      = 0           -- z

-- Emote 

    Set['Con_2_Emote']      = true        -- Active true/false
    Set['Con_2_EffectID']   = 4           -- Effect ID 
    Set['Con_2_EffectType'] = "chicken"    -- The Effect Name
    Set['Con_2_ETarget']    = true        -- Turns the NPC to the Target Before Emoting 
    Set['Con_2_EDelay']     = 8.75        --  Delay (Min should be the animation time)
    Set['Con_2_ESkill']     = false
    Set['Con_2_ESkillID']   = 31    
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
    Set['Pet_SpawnID']          = 170  -- Spawner ID [1802 = Pirate] 
    Set['Pet_Count']            = 2     -- Number of pets the given parent can have
    Set['Pet_Respawn']          = 10    -- Pet Respawn time 
    Set['Pet_DisMin']           = 10     -- Raduis Min Distance
    Set['Pet_DisMax']           = 20    -- Raduis Max Distance
    Set['Pet_Follow']           = false -- Pet will follow the Parent
    Set['Pet_FollowDis']        = 5     -- Distance to follow 

    --  Not working right now --
    Set['Pet_HealthLink']       = false
    Set['Pet_UseEffect']        = false
    Set['Pet_SpawnEffectID']    = 4
    Set['Pet_SpawnType']        = "name"

    Set['Pet_Child']            = false
    Set['Pet_ChildMovement']    = "follow"   
 --[[ 
--/////////////////////////////////////////////////////////////////////////

            _       ____   _____ 
           / \     / ___| |_   _|
          / _ \   | |       | |  
         / ___ \  | |___    | |  
        /_/   \_\  \____|   |_|         
         
--]]

    Set['Act_Active']           = false
    Set['Act_SpawnInMode']      = "emoteBreak"  -- ["broken"], ["emoteBreak"],     
    Set['Act_Spawn']            = 170 -- activitie to spawn 
    Set['Act_Reset']            = 120   -- time in Idle state before reseting. 



------ Due not change --------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
-------------------------------------------------------------------------------

end













