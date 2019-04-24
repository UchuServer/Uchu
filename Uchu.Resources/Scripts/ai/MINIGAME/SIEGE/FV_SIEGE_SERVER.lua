require('o_mis')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_EVENTS')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_STATES')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_PLAYERLOADED')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_PLAYERDIED')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_SWAPTEAMS')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_NOTIFYOBJECT')
require('/ai/MINIGAME/SIEGE/SERVER/SIEGE_REWARDS')

function onStartup(self) 
Set = {}
	-- Basic Game Settings --
	--(((((((((((( 1.	Enter Level: wait for other players )))))))))))))
	Set['GameState'] = "Starting"					-- Do Not Change -- 
    Set['Number_Of_Teams']          = 2             -- INT ( Set the number of teams )
    Set['Number_Of_PlayersPerTeam'] = 4             -- INT ( Set the number of players on each team )
    Set['Rounds_To_Play']           = 2             -- INT ( Set the number of rounds to play ) 
    Set['Minimum_Players_to_Start'] = 2			-- INT ( The min number of players to start game )
    Set['Score_To_Complete']        = 3             -- INT ( Set Score to complete a round )
    Set['RespawnTime']              = 5			-- INT ( Player Respawn Time after being smashed )
    Set['Game_Type']                = "SIEGE"  		-- INT ( Game Type )
	
	Set['Team_A_Name'] = "Ninjas"
	Set['Team_B_Name'] = "Pirates"

	Set['Join_Timer'] = 500 

    -- Siege Timers Repeats before each round -- 
    Set['Notify_Team_Objectives'] = 1				-- INT(Duration of Help Screen at start of game & between each round)
    Set['GameStart_CountDown']  = 10				-- INT(Duration of the countdown at the beginning of the game)
	Set['DefendTime'] = 300								-- INT(Duration of the round)
	-- End of Round Timers -- 
	Set['State_Results_Timer'] = 15					-- INT(Duration of the Scoreboard at the end of each round)
	Set['Barrel_Reset'] = 10								-- INT(Time that the barrel takes to return to its spawn after being dropped)
     
	Set['Barrel_Speed'] = 350
	
	-- Siege Objects Settings -- 
	Set['QB_Object_Group']    = "qb_objects"		-- STRING ( Group name of the QB objects )	
	Set['QB_Oject_Time']    = 10					-- INT ( Build time of rebuild objects ) 
	Set['QB_Oject_Health']  = 5						-- INT ( Heath of the QB )
	Set['QB_Object_LOT']    = 6620					-- LOT ( LOT # of the Main object the attackers are building )	
	Set['QB_Loot_Object']   = 6600
	Set['Gate_Object']    = 6484					-- LOT ( LOT # of Gate)	
    
	-- Siege Points
	Set['Info_Text_1'] = " smashed "
	Set['Info_Text_2'] = " joined the siege!"
	Set['Info_Text_3'] = " left the siege."
	Set['Info_Text_4'] = " picked up a barrel!"
	Set['Info_Text_5'] = " dropped a barrel!"
	Set['Info_Text_6'] = " returned a barrel!"
	Set['Info_Text_7'] = " captured a barrel!"
	
	-- Round End Messages       <<<------Trent add support for these messages
	Set['Info_Text_8'] = "Your team lost the Round"
	Set['Info_Text_9'] = "Your team won the Round"
	Set['Info_Text_10'] = "Your team lost the Match"
	Set['Info_Text_11'] = "Your team won the Match"
	
	-- All Vars are * 1 
	Set['CapturObj'] = 10
	Set['PickUpObj'] = 5		
	Set['Deaths'] = 		0
	Set['Kills'] =          1		
	Set['Build'] = 			1
	Set['DestroyQB'] = 1
	Set['KillObjCarrier'] = 1
	Set['RetrunObj'] = 5	
	Set['WonMatchMultiplier'] = 100	
	
	-- End Game Timers 
	Set['WonLoastMatchTimer'] = 5    -- Show Txt Won Lost timer
	Set['ScoreBoardTimer'] = 5    -- Show Score Board Timer
	Set['LeaderBoardTimer'] = 5
	
	--((((((((((((  2.	Notify Team Objectives:  ))))))))))))) -- 
	Set['Notify_Txt_Team_1'] = "Defend the rebuilds."
	Set['Notify_Txt_Team_2'] = "Attack and return"


   --((((((((((((  4.Prestart Game timer for the Defenders. ))))))))))))) -- 
	Set['Prestart_Time'] = 30
	Set['Prestart_Txt_Team_1'] = "You have "..tostring(Set['Prestart_Time']).." seconds to set up defense"
	Set['Prestart_Txt_Team_2'] = "You have "..tostring(Set['Prestart_Time']).." seconds before attacking"
  
  	-- SEIGE Object Groups -- 
  	
    Set['Gate_Group'] = "grp_gate"
    Set['Wall_Group'] = "grp_walls"
    Set['Mine_Group'] = "grp_mines" 
  
    -- Player Settings -- 
    Set['CustomPlayer'] = false    -- BOOL
    Set['Health']       = 4      -- INT
    Set['Armor']        = 2       -- INT
    Set['Imagination']  = 8       -- INT
    
   	 -- Team Settings --
	 -- Custom Team Settings color and skills  
	 -- Colors are in the database  ( dbo_BrickColors )    
    Set['Use_Themes']     = true -- Bool
	Set['Use_Skills']	  = false -- Bool  
    --- Team 1 ---
    Set['Team_Color_1']      = 1 -- 
    --- ATK Skills ---
    Set['Team_Skill_A_1']    = nil --    
    Set['Team_Skill_B_1']    = nil --     
    Set['Team_Skill_C_1']    = nil --     
    Set['Team_Skill_D_1']    = nil --    
	Set['Team_Skill_E_1']    = nil --    
    --- Team 2 ---
    Set['Team_Color_2']      = 0 --   
    --- ATK Skills ---
    Set['Team_Skill_A_2']    = nil --    
    Set['Team_Skill_B_2']    = nil --     
    Set['Team_Skill_C_2']    = nil --     
    Set['Team_Skill_D_2']    = nil --    
	Set['Team_Skill_E_2']    = nil -- 

	--- Game Object Lots ---
	Set['Number_of_Spawn_Groups'] = 1 --INT
    Set['Red_Spawners'] = 4847
    Set['Blue_Spawners'] = 4848
    Set['Blue_Flag'] = 4850
    Set['Red_Flag'] = 4851
    Set['Red_Point'] = 4846
    Set['Blue_Point'] = 4845
    Set['Red_Mark'] = 4844
    Set['Blue_Mark'] = 4843
    Set['WhatTeam'] = "A"
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    self:SetNetworkVar("Set",Set)
    oStart(self)
end

function onNotifyObject(self, msg)
    if( msg) then
        mainNotifyObject(self, msg)
    end
end

function onObjectLoaded(self, msg)
    if (msg) then
        mainObjectLoaded(self, msg)
    end
end

function onChildLoaded(self, msg)
    if (msg) then
        mainChildLoaded(self, msg)
    end
end
--------------------------------------------------------------------------------
