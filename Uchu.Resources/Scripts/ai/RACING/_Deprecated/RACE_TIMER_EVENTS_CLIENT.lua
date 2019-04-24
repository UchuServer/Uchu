

function ActivityTimers(self, player, name , endRace , place )
 	local playerName = player:GetName().name
 	

 	if name == "StartTotalTime" then
 	
 		-- Start Timers 
 		self:ActivityTimerSet{name = playerName.."_TotalTime",updateInterval = 0.1 }
 		self:ActivityTimerSet{name = playerName.."_LapTime"  ,updateInterval = 0.1 }
 		GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "test" , self )
 		
	elseif name == "StartNewLapTime"  then
	
		local Ltime = string.format("%g",self:ActivityTimerGet{name = playerName.."_LapTime"}.timeElapsed)
		local Laptime = self:ActivityTimerGet{name = playerName.."_LapTime"}.timeElapsed
		local MiliSec =  string.sub(string.gsub(string.format("%f",Laptime), ".", "|",1),3, 3)
		local LapNumber = self:GetActivityUserData{ userID = player, typeIndex = 3}.outValue
		
        if not (MiliSec) then
            MiliSec = tostring(5)
        end
    
		-- Store Lap Time
		self:SetVar("LapTime_"..self:GetVar("Con.Current_Lap"), Laptime )

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

		-----------------------------------------------------------
		-- Show Lap Time to Driver	
		-----------------------------------------------------------
		local RTime = SecondsToClock(Laptime) 
		local Mili  = tostring(tonumber(MiliSec))
		UI:SendMessage("RaceHud", {{"showtext", true } ,{"stColor", "0xFFFFFF" } ,{"stSize", "36" } ,{"sttext", RTime..":0"..Mili },{"sttime", "10" } } )
		-----------------------------------------------------------
		-- Stop Timer
		-----------------------------------------------------------
		self:ActivityTimerStop{name = playerName.."_LapTime"}
		-----------------------------------------------------------
		-- Update Lap Number
		-----------------------------------------------------------
		UI:SendMessage("RaceHud", {{"race_current_lap", tostring(LapNumber) }} )
	    self:SetVar("Con.Current_Lap", LapNumber ) 
	
		if (endRace == "false") then
			-----------------------------------------------------------
			-- ReStart Timer
			-----------------------------------------------------------
			self:ActivityTimerSet{name = playerName.."_LapTime"  ,updateInterval = 0.1 }
		
		else
			self:ActivityTimerStop{name = playerName.."_TotalTime"}
		    self:ActivityTimerStop{name = playerName.."_LapTime"}
		end
		
	end
	
end



function onActivityTimerUpdate(self, msg)

	local objects = self:GetAllActivityUsers{}.objects

	for i = 1, #objects do
		
		local player = objects[i]
		
		if (player) then
		
			local time = msg.timeElapsed
			
		
			local playerName = player:GetName().name
			if msg.name == playerName.."_TotalTime" then
			
			
			UI:SendMessage("RaceHud", {{"totallaptime", SecondsToClock(time) }} )	
			
			end


			if msg.name == playerName.."_LapTime" then
		
				
		
			UI:SendMessage("RaceHud", {{"laptime", SecondsToClock(time) }} )	
				
			end
			

		end
	end

end
  