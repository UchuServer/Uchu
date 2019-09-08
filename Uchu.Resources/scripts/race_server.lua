require('o_mis')
require('RACE_EVENTS')
require('RACE_NOTIFYOBJECT')
require('RACE_STATES')
require('RACE_TIMER_EVENTS')
require('RACE_ACTIVITY')
require('RACE_ACTIVITY_SERVER')

function onStartup(self) 
Set = {}
	-- Basic Game Settings --
	Set['GameType'] = "Racing"
	Set['GameState'] = "Starting"					-- Do Not Change -- 
    Set['Number_Of_PlayersPerTeam'] = 8            -- INT ( Set the number of players on each team )
    Set['Minimum_Players_to_Start'] = 2				-- INT ( The min number of players to start game )

	--- Game Object Lots ---
	Set['Car_Object'] = 6990
	Set['Race_PathName'] = "MainPath"
	Set['Current_Lap'] = 1
	Set['Number_of_Laps'] = 1			   -- Number of Laps to complete the Race                     
	Set['activityID'] = 39
	
	Set['Place_1'] = 100
	Set['Place_2'] = 90
	Set['Place_3'] = 80
	Set['Place_4'] = 70
	Set['Place_5'] = 60
	Set['Place_6'] = 50
	Set['Place_7'] = 40
	Set['Place_8'] = 20
	
	
	-- Reward % Rating --
	Set['Num_of_Players_1'] = 15		
	Set['Num_of_Players_2'] = 25
	Set['Num_of_Players_3'] = 50
	Set['Num_of_Players_4'] = 75
	Set['Num_of_Players_5'] = 80
	Set['Num_of_Players_6'] = 90
	Set['Num_of_Players_7'] = 95
	Set['Num_of_Players_8'] = 100


	
	Set['Number_of_Spawn_Groups'] = 1 --INT
    Set['Red_Spawners'] = 4847
    Set['Blue_Spawners'] = 4848
    Set['Blue_Flag'] = 4850
    Set['Red_Flag'] = 4851
    Set['Red_Point'] = 4846
    Set['Blue_Point'] = 4845
    Set['Red_Mark'] = 4844
    Set['Blue_Mark'] = 4843
    
    Set['CarColor_1'] = 21  -- Bright Red
    Set['CarColor_2'] = 23  -- Bright Blue
    Set['CarColor_3'] = 24  -- Bright Yellow
    Set['CarColor_4'] = 28  -- Dark Green
    Set['CarColor_5'] = 268 -- Medium lilac
    Set['CarColor_6'] = 106 -- Bright Orange
    Set['CarColor_7'] = 308 -- Dark Brown
    Set['CarColor_8'] = 26  -- Black
    
    
	-- Do not change ----------------------------------------------------------
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
