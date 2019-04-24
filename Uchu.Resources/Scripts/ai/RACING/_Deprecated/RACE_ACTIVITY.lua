----------------------------------------------------------------
-- Sets up an activity of the object, only need to put in variable you want to change
----------------------------------------------------------------
function SetupActivity(self,nMaxUsers) 

	-- set max users to something high
	self:SetActivityParams{  modifyMaxUsers = true, maxUsers = nMaxUsers, modifyActivityActive = true,  activityActive = true} 
end
----------------------------------------------------------------
-- Returns true/false if a player is in the activity
-- takes self and a PLAYER object
----------------------------------------------------------------
function IsPlayerInActivity(self,player)
  
	-- check if player is in activity
	local existMsg = self:ActivityUserExists{ userID = player }
	if (existMsg) then
		return existMsg.bExists
	end
	return false
   
end
----------------------------------------------------------------
-- Add Player and Car to activity: Object
----------------------------------------------------------------
function AddPlayerAndCarToActivity(self,player, carID )

	 if (player) and (carID:GetType{}.objType == "Vehicle") and not IsPlayerInActivity(self,player) then
		self:AddActivityUser{ userID = player }
		self:SetActivityUserData{userID = player ,controlledID =  carID, typeIndex = 0, value = 0 }
		
		
		
		
	 end
	 if (player) and (carID) then
		 SetActivityValue(self, player, 0, 0  )
	
		  

	 end	
	
end

----------------------------------------------------------------
-- adds the valueVar to the existing value of the index for the 
-- player in the activity 
----------------------------------------------------------------
function UpdateActivityValue(self, player, valueIndex, valueVar , carID)
 
    local newValue = self:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue + valueVar
    SetActivityValue(self, player, valueIndex, newValue  )
 
end

----------------------------------------------------------------
-- Stores a vaariable for the player in the activity
----------------------------------------------------------------
function SetActivityValue(self, player, valueIndex, valueVar  )

	local carID = getObjectByName(self, player:GetName().name.."CarID")
    self:SetActivityUserData{ userID = player, controlledID = carID ,typeIndex = valueIndex, value = tonumber(valueVar) }
    
end

----------------------------------------------------------------
-- Gets the value of the activity index for the player
----------------------------------------------------------------
function GetActivityValue(self, player, valueIndex)
  
    return self:GetActivityUserData{ userID = player, typeIndex = valueIndex}.outValue
  
end

----------------------------------------------------------------
-- StopActivity message
----------------------------------------------------------------
function StopActivity(self,player, scoreVar, value1Var, value2Var, quit , carID)
  

    -- user is trying to cancel
		if quit then

			-- remove the user from activity
			self:RemoveActivityUser{ userID = player }        

		else    
		
			SetActivityValue(self, player, 0, scoreVar  )
			
			if value1Var ~= nil then            
		
				SetActivityValue(self, player, 1, scoreVar  )
			elseif value2Var ~= nil then        

				SetActivityValue(self, player, 2, scoreVar  )
			end

			if (self) then
			
				SetActivityValue(self, player, 0, scoreVar  )
				if value1Var ~= nil then            
					SetActivityValue(self, player, 1, value1Var  )
				elseif value2Var ~= nil then        
	
					SetActivityValue(self, player, 2, value2Var  )
				end
			
			end

		    --------------------------------------------------------------------
			-- distribute rewards  
            --------------------------------------------------------------------     
			self:DistributeActivityRewards{ userID = player, bAutoAddCurrency = true, bAutoAddItems = true }   -- debug in C++   

			-- Update Leaderboards for this user
			 self:UpdateActivityLeaderboard{ userID = player }

			-- remove the user from activity
			--self:RemoveActivityUser{ userID = player } 
			
			
			--- Update reward UI ( send vars to client )
			local reward = self:GetActivityReward{playerID = player}
			local rewardItem1Image = reward.rewardItem1Image
			local rewardItem1Name = reward.rewardItem1Name
			local rewardItem1StackSize = reward.rewardItem1StackSize
			local rewardItem2Image = reward.rewardItem2Image
			local rewardItem2Name = reward.rewardItem2Name
			local rewardItem2StackSize = reward.rewardItem2StackSize
			local rewardMoney = reward.rewardMoney
			
			local rewardString =  rewardMoney..","..rewardItem1Name..","..rewardItem1Image..","..rewardItem1StackSize..","..rewardItem2Name..","..rewardItem2Image..","..rewardItem1StackSize
			
			 self:NotifyClientZoneObject{ name = "RewardPlayer" , paramStr = rewardString,  rerouteID = player   }
			 
		end
   
end


----------------------------------------------------------------
-- GetLeaderboard Data message
----------------------------------------------------------------
function GetLeaderboardData( self, player, activityID )
 
    -- get the leaderboard data for the user and update summary screen if it exists
    self:RequestActivitySummaryLeaderboardData{ user = player, target = self, queryType = 7, gameID = activityID } 
  
end

--------------------------------------------------------------
-- Calculate the activity rating
--------------------------------------------------------------
function onDoCalculateActivityRating(self, msg)
 
	

	msg.outActivityRating = 84
	return msg
	
end


---------------------------------------------------------------------------------------
--  Sent to the racing control object when a player's rank in the race has changed.
---------------------------------------------------------------------------------------
function onRacingPlayerRankChanged(self, msg) 

	local player = msg.playerID
	local oldRank = msg.oldRank
	local newRank = msg.newRank
	local carID = getObjectByName(self, player:GetName().name.."CarID")

	
			
		self:SetActivityUserData{ userID = player, typeIndex = 0,controlledID = carID, value = oldRank }
	

end

---------------------------------------------------------------------------------------
--  player crosses the finish line.
---------------------------------------------------------------------------------------
function onRacingPlayerCrossedFinishLine(self, msg)

	local player = msg.playerID
	local oldlap = msg.oldLap
	local newLap = msg.newLap

	local PlayerPos = self:GetActivityUserData{ userID = player, typeIndex = 0}.outValue
	local LapScore = self:GetActivityUserData{ userID = player, typeIndex = 3}.outValue

	SetActivityValue(self, player, 3,  LapScore + 1   )

	if (self:GetActivityUserData{ userID = player, typeIndex = 3}.outValue == self:GetVar("Set.Number_of_Laps")  + 1)then

		---------------------------------------------------------------------------------------
		--  Driver Completed the Race
		--  Lock Brake
		--  Flag to Completed
		---------------------------------------------------------------------------------------
		local car =  getObjectByName(self, player:GetName().name.."CarID" )
		
		car:VehicleLockInput{bLockWheels = true}
		local pos = self:GetActivityUserData{ userID = player, typeIndex = 0}.outValue
		ServerActivityTimers(self,  player, "StartNewLapTime" ,true , pos )
		self:NotifyClientZoneObject{name = "ActivityTimers" , paramObj = player , paramStr = "StartNewLapTime,true", param2 = oldlap,  rerouteID = player  }
		

	else

		---------------------------------------------------------------------------------------
		--  Driver Crossed Finish Line Race not yet Completed
		---------------------------------------------------------------------------------------
		
		ServerActivityTimers(self, player, "StartNewLapTime" ,false , PlayerPos)
		self:NotifyClientZoneObject{name = "ActivityTimers" , paramObj = player , paramStr = "StartNewLapTime,false", param2 = oldlap,  rerouteID = player  }

	end
	
end


---------------------------------------------------------------------------------------
--   Lets racing control object know when a player has left the track's 
---------------------------------------------------------------------------------------
--function onRacingPlayerOutOfTrackBounds(self, msg)

--		local player = msg.playerID
--		local carID = getObjectByName(self, player:GetName().name.."CarID")

--		 if (carID:VehicleCanWreck().bCanWreck == true) then

--				carID:Die{ killType = "VIOLENT" }   
--		 end
	

	
--end

---------------------------------------------------------------------------------------
--   Message to script Player is heading the wrong way
---------------------------------------------------------------------------------------

function onRacingPlayerWrongWayStatusChanged(self, msg)

	local player = msg.playerID
	local way = msg.bGoingWrongWay	
	if (way) then
	 self:NotifyClientZoneObject{name = "WrongWayTrue"  , rerouteID = player  }
	else
	 self:NotifyClientZoneObject{name = "WrongWayFalse"  , rerouteID = player  }
	
	end
	
end
