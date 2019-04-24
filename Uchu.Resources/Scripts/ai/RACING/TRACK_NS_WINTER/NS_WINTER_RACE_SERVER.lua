require('ai/RACING/RACE_IMAGINATION_SPAWNER')
require('o_mis')


function onStartup(self)
	--print("RACING: onStartup")
	  
    -- configure the racing control
    local racingParams =
		{
			{ "GameType", "Racing" },
			{ "GameState", "Starting" },					-- Do Not Change --
			{ "Number_Of_PlayersPerTeam", 6 },				-- INT ( Set the number of players on each team )
			{ "Minimum_Players_to_Start", 2 },				-- INT ( The min number of players to start game )
			{ "Minimum_Players_for_Group_Achievments", 2 },	-- INT ( the minimum number of players required to get "group" achievements )

			--- Game Object Lots ---
			{ "Car_Object", 7703 },
			{ "Race_PathName", "MainPath" },
			{ "Current_Lap", 1 },
			{ "Number_of_Laps", 3 },		   -- Number of Laps to complete the Race
			{ "activityID", 42 },

			{ "Place_1", 100 },
			{ "Place_2", 90 },
			{ "Place_3", 80 },
			{ "Place_4", 70 },
			{ "Place_5", 60 },
			{ "Place_6", 50 },

			-- Reward % Rating --
			{ "Num_of_Players_1", 15 },
			{ "Num_of_Players_2", 25 },
			{ "Num_of_Players_3", 50 },
			{ "Num_of_Players_4", 85 },
			{ "Num_of_Players_5", 90 },
			{ "Num_of_Players_6", 100 },

			{ "Number_of_Spawn_Groups", 1 }, --INT
			{ "Red_Spawners", 4847 },
			{ "Blue_Spawners", 4848 },
			{ "Blue_Flag", 4850 },
			{ "Red_Flag", 4851 },
			{ "Red_Point", 4846 },
			{ "Blue_Point", 4845 },
			{ "Red_Mark", 4844 },
			{ "Blue_Mark", 4843 },
						
		}
	--print("ConfigureRacingControl...")
	self:ConfigureRacingControl{ parameters = racingParams }
	--print("...Done")
end

function onSetActivityUserData(self, msg)

	local playerID = msg.userID:GetID()
	local existing = self:GetVar(playerID)
	
	if not existing then
		local takeCost = self:ChargeActivityCost{user = msg.userID}.bSucceeded
		self:SetVar(playerID, true)
	end
end

--------------------------------------------------------------------------------
