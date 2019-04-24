function ServerActivityTimers(self, player, name , endRace , place )

 	local playerName = player:GetName().name
 	
 	if name == "StartTotalTime" then
 	
 		-- Start Timers 
 		self:ActivityTimerSet{name = playerName.."_TotalTime",updateInterval = 0.1 }
 		self:ActivityTimerSet{name = playerName.."_LapTime"  ,updateInterval = 0.1 }
	---------------------------------------------------------------------------------------
	--   Start a new Lap with timers
	--   Get time Vars
	--   Saves Lap time in a Table with best Lap
	--   Start a new lap time
	--   Update Clients GUI with times
	--------------------------------------------------------------------------------------- 
	elseif name == "StartNewLapTime"  then
		local Ltime = string.format("%g",self:ActivityTimerGet{name = playerName.."_LapTime"}.timeElapsed)
		local Laptime = self:ActivityTimerGet{name = playerName.."_LapTime"}.timeElapsed
		local MiliSec =  string.sub(string.gsub(string.format("%f",Laptime), ".", "|",1),3, 3)
		local LapNumber = self:GetActivityUserData{ userID = player, typeIndex = 3}.outValue
		
        if not (MiliSec) then
            MiliSec = tostring(5)
        end
    
		-- Store Lap Time
		self:SetVar("LapTime_"..self:GetVar("Set.Current_Lap"), Laptime )

		-- Store Table Lap Time for Best Time -- 
		
		if self:GetVar(playerName.."_LastTime") == nil then
			--print("Lap Time Set too  ".. Laptime )
			self:SetVar(playerName.."_LastTime", Laptime) 
			self:SetVar(playerName.."_LastMili", MiliSec) 
			
			self:SetVar(playerName.."_float_BestLap", Laptime )
		
		end

		if self:GetVar(playerName.."_LastTime") > Laptime then
		
			self:SetVar(playerName.."_LastTime", Laptime) 
			self:SetVar(playerName.."_LastMili", MiliSec) 
			--print("New Best Time  ".. Laptime )
		
		end


		-- Show Lap Time to Driver		
		 self:NotifyClientZoneObject{ name = "ShowLapTime", param1 = Laptime , paramObj = player,  param2 = tonumber(MiliSec) , rerouteID = player}
		-- Stop Timer
		self:ActivityTimerStop{name = playerName.."_LapTime"}
		-- Update Lap Number
		self:NotifyClientZoneObject{ name = "LapNumber", param1 = LapNumber, rerouteID = player}
	
		if (endRace == false) then
			-- ReStart Timer
			self:ActivityTimerSet{name = playerName.."_LapTime"  ,updateInterval = 0.1}
		end
		
		---------------------------------------------------------------------------------------
		--   Completed Race
		---------------------------------------------------------------------------------------  
		if  (endRace == true) then
			
			self:SetVar(playerName.."_TotalTime", self:ActivityTimerGet{name = playerName.."_TotalTime"}.timeElapsed )
			self:SetVar(playerName.."_float_TotalTime", self:ActivityTimerGet{name = playerName.."_TotalTime"}.timeElapsed )
			
			local finalPos = self:GetActivityUserData{ userID = player, typeIndex = 0}.outValue
			local BestTime =  SecondsToClock(self:GetVar(player:GetName().name.."_LastTime"))..":0"..self:GetVar(player:GetName().name.."_LastMili")
			local TotalTimer = ""..SecondsToClock(self:GetVar(player:GetName().name.."_TotalTime"))..":0"..self:GetVar(player:GetName().name.."_LastMili")..""
	
			-- if one player mark a 1st place --
            if  (#self:GetAllActivityUsers{}.objects == 1) then
                SetActivityValue(self, player, 0,  1  )
                finalPos = 1
            end
			
			self:ActivityTimerStop{name = playerName.."_TotalTime"}
            -- Set Player Pos
			playerFinished(self , player , ""..playerName..","..BestTime..","..TotalTimer..","..finalPos.."")
			
			-- notify the racing control component - this player has finished the race
			self:RacingPlayerEvent{ eventType="FINISHED_RACE", playerID=player }
		
		end
	
	    elseif name == "EndRace" then
	

	
	end
	
end
---------------------------------------------------------------------------------------
--   Driver Finished Save Stats to Server and Client
--------------------------------------------------------------------------------------- 
function playerFinished(self, player , dTimes )

	    local playerName = player:GetName().name
	    local final = split(dTimes,",") 

		
		-- Save Player Race Stats
		self:SetVar( playerName.."_Stats", "Finished" )
		self:SetVar( playerName.."_Name", final[1])
		self:SetVar( playerName.."_BestTime", final[2] )
		self:SetVar( playerName.."_TotalTimer", final[3] )
		self:SetVar( playerName.."_finalPos", final[4] )
		
		-- Notify Client Player Finished --
		self:NotifyClientZoneObject{ name = "Driver_Finished" , paramObj = player }


		-- Hide Boost -- Hide Gage -- Hide Stats -- *GUI*
		self:NotifyClientZoneObject{ name = "RaceFinishHideUI" , rerouteID = player}
		
		self:NotifyClientZoneObject{ name = "place_"..final[4] , paramStr = dTimes , paramObj = player }

		
		-- Set Timer to Update other Drivers
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "Fline", self )

		rewardPlayer(self, player, pos )

end
---------------------------------------------------------------------------------------
--  Reward Player
--------------------------------------------------------------------------------------- 
function rewardPlayer(self, player )

  	local playerName = player:GetName().name
    local bestLap = self:GetVar(playerName.."_float_BestLap")
    local TotalTime = self:GetVar(playerName.."_float_TotalTime")
	local CarID = getObjectByName(self, player:GetName().name.."CarID") 	
	local pos = self:GetActivityUserData{ userID = player, typeIndex = 3}.outValue - 1


	UpdateActivityValue(self,player, 1, TotalTime , CarID)
	UpdateActivityValue(self,player, 2, pos       , CarID)
	--UpdateActivityValue(self,player, 3, pos       , CarID)
	 	
	StopActivity(self,player, TotalTime,0,0, false ,CarID) 

end

 