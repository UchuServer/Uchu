 --------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--carID:NotifyZoneObject{ name = "updateCarNumber" , ObjIDSender = player, param1 = oldlap , param2 = newLap }

--require('/ai/RACING/SERVER/RACE_ACTIVITY_CLIENT')
--require('/ai/RACING/SERVER/RACE_TIMER_EVENTS_CLIENT')


function not_onStartup(self)
    Con = {}
    Con["Current_Lap"] = 1
	self:SetVar("Con",Con)
	
	UI:SendMessage("ShowUI", { {"show", true } })
	UI:SendMessage("ChatBoxVisible", { {"chatvisible", "show" } })
	
	for i = 1, 8 do
	
		self:SetVar("racePos_"..i, 0)
		
	end
end

function  not_onChildLoaded(self, msg)
	 

	if (msg.childID:GetType().objType == "Vehicle" ) then 
	
		local Child = msg.childID
		local objects = self:GetAllActivityUsers{}.objects
		
		for i = 1, #objects do
			local player =  objects[i]
				
				if Child:GetID() == getObjectByName(self, player:GetName().name.."CarID"):GetID() then
				 -- print("Child added ****************** "..player:GetName().name)
				  self:AddActivityUser{ userID = player }
				  self:SetActivityUserData{userID = player ,controlledID =  Child, typeIndex = 0, value = 0 }
				  player:PlayCinematic { pathName = "P1" } 
				  break
				end

			end
		end
    
end

function not_onNotifyClientZoneObject(self,msg) 
	
	if msg.name == "ShowPlayButton" then
	
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "showReadyButton", self )
	elseif msg.name == "RewardPlayer" then
	
			UI:SendMessage("Race_Reward", {{"RewardPlayer", msg.paramStr }} )
			--print("Rewards to GUI ".. msg.paramStr)
		
	elseif msg.name == "exit" then
		
		UI:SendMessage("Leaderboard", {{"leaderboardUI", "hide" }} )
	elseif msg.name == "replay" then
	
		UI:SendMessage("Leaderboard", {{"leaderboardUI", "hide" }} )
		
	elseif msg.name == "StartRace" then
		UI:SendMessage("RaceJoin", {{"UI", "hide" }} )

	elseif msg.name == "pushStateRace" then
		UI:SendMessage( "pushGameState", {{"state", "Race" }} )
	elseif  msg.name == "pushStateGamePlay" then
	
		UI:SendMessage( "pushGameState", {{"state", "gameplay" }} )
	elseif msg.name == "PlayerReady" then
		UI:SendMessage("RaceJoin", {{"playerready", msg.paramStr }} )
		
	elseif msg.name == "removeplayer" then
		UI:SendMessage("RaceJoin", {{"removeplayer", msg.paramStr }} )
		
	elseif msg.name == "EnableCheck" then
	
		for x = 1, #self:GetAllActivityUsers{}.objects do 	
			if msg.paramStr == "name_p"..x.."_t1" then		
				UI:SendMessage("RaceJoin", {{"EnableCheck", msg.paramStr }} )
				
				--print("EnableCheck")
			end	
		end	
		
	elseif msg.name == "JoinTeam" then

		for x = 1,# self:GetAllActivityUsers{}.objects  do 
			if msg.paramStr == "name_p"..x.."_t1" then		
				UI:SendMessage("RaceJoin", {{"join_name", msg.paramStr },{ "playername", msg.paramObj:GetName().name }} )
				--print("JoinTeam")
			end	
		end
		
	elseif msg.name == "ClearUI" then
		UI:SendMessage("RaceJoin", {{"clearGUI", true }} )
		UI:SendMessage("RaceJoin", {{"UI", "show" }} )
		
	elseif msg.name == "SetPlayerName" then
		UI:SendMessage("RaceJoin", {{"user", msg.paramObj  }} )
		
	elseif msg.name == "StoreClientPlayer" then
		UI:SendMessage("RaceJoin", {{"clearGUI", true }} )
		UI:SendMessage("RaceJoin", {{"UI", "show" }} )	

	elseif msg.name == "sendTo_Team_1" then 
		local objects = #self:GetAllActivityUsers{}.objects
		for x = 1, objects  do  
			local player = objects[x]
			local text = msg.paramStr
		   -- --print(text)
			player:DisplayTooltip { bShow = true, strText = text, iTime = 2000 }
		end
		
	-- Set Race HUD GUI
	elseif msg.name == "reSetRaceGUI" then
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		
		-- Leader Board
		UI:SendMessage("Leaderboard", {{"leaderboardUI", "hide"}} )
		UI:SendMessage("Leaderboard", {{"leaderboardUI", "hidebutton"}} )
		
		-- Reward
		UI:SendMessage("Race_Reward", {{"RewardUI", "hide"}} )
		
		-- Stat
		UI:SendMessage("RaceStat", {{"raceStatUI", "show"}} )
		
		
		-- boost
		UI:SendMessage("RaceBoost", {{"boostUI", "show"}} )
		
		-- Standings
		UI:SendMessage("RaceStanding", {{"StandingUI", "hide" }} )
		UI:SendMessage("RaceStanding", {{"LockUI", "unlock" }} )
		
		-- Hud
		UI:SendMessage("RaceHud", {{"UI", "show" }} )
		UI:SendMessage("RaceHud", {{"race_current_lap", "1" }} )	
		UI:SendMessage("RaceHud", {{"race_remaining_lap", msg.paramStr }} )
		UI:SendMessage("RaceHud", {{"resetUI", true } })
	
		-- Gage
		UI:SendMessage("RaceGage", {{"raceGageUI", "show"}} )
		UI:SendMessage("RaceGage", {{"resetmph", true }} )
	

		 		
	elseif msg.name == "SetDriverPos" then

		UI:SendMessage("RaceStat", {{"racePos_"..msg.param1, msg.paramStr }} )
		self:SetVar("racePos_"..msg.param1, msg.paramStr)
		
	elseif msg.name == "StartCountDown" then
		
		UI:SendMessage("RaceCount", {{"Count",true }} )
			
	elseif msg.name == "FreezPlayer" then
	
		local eChangeType = "PUSH"
		msg.paramObj:SetStunned{ StateChangeType = eChangeType,
					bCantMove = true, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }	
	elseif msg.name == "ActivityTimers" then
	
		local t = split(msg.paramStr, ',')
		ActivityTimers(self, msg.paramObj, t[1] , t[2] , msg.param2 )
		self:SetVar(msg.paramObj:GetName().name,"running")
		
		
	  
    		  
	elseif msg.name == "ActivityStart" then
		     self:ActivityStart()
		     self:SetVar("RaceStarted", true ) 
		     
		         
	elseif msg.name == "SetPath" then
	
		self:SetCurrentPath{pathName = msg.paramStr}
		
	elseif msg.name == "WrongWayTrue" then
	
			UI:SendMessage("RaceWW", {{"UI","show" }} )
	elseif msg.name == "WrongWayFalse" then
	
			UI:SendMessage("RaceWW", {{"UI","hide" }} )
			
	elseif msg.name == "SetActivityParams" then
	
		 self:SetActivityParams
		 { 
			 activityID = msg.param1 ,  
			 modifyMaxUsers = true, maxUsers = 8, 
			 modifyActivityActive = true,  activityActive = true
		 }
			
	 
	elseif msg.name == "LapNumber" then
		UI:SendMessage("RaceHud", {{"race_current_lap", tostring(msg.param1) }} )
	elseif msg.name == "rewardGUI" then
		
		UI:SendMessage("RaceStanding", {{"LockUI", "lock" }} )
		UI:SendMessage("Race_Reward", {{"RewardUI", "show"}} )
	elseif msg.name == "Driver_Finished" then
	
		local player = msg.paramObj	
	
		self:SetVar(player:GetName().name.."_Stats", "Finished")
	elseif msg.name == "LockUI" then
	
		UI:SendMessage("RaceStanding", {{"LockUI", "lock" }} )
	elseif msg.name == "unLockUI" then
	
		UI:SendMessage("RaceStanding", {{"LockUI", "unlock" }} )	
	elseif msg.name == ("RaceFinishHideUI") then
		UI:SendMessage("RaceGage", {{"raceGagetUI", "hide"}} )
		UI:SendMessage("RaceBoost", {{"boostUI", "hide"}} )
		UI:SendMessage("RaceStat", {{"raceStatUI", "hide"}} )
	elseif msg.name == "storePlayersCarID" then
	
	    storeObjectByName(self, msg.paramStr, msg.paramObj )
	   
	end	
	
	
	  local objects = #self:GetAllActivityUsers{}.objects

	  for i = 1, objects do
		
		if msg.name == "place_"..i then

			 local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
			 local s = msg.paramStr
			 local t = split(s, ',')
			
			 if self:GetVar(player:GetName().name.."_Stats") == "Finished" then
	
				 UI:SendMessage("RaceHud", {{ "UI" ,"hide" }} )
				 UI:SendMessage("RaceStanding", {{ "place_"..i ,true },{"placeName",t[1] },{"bestlaptime", t[3] },{"totaltime",t[2] }} )	
			 end

		end

	 end

				
		
			
 	  
end



not_onTimerDone = function(self, msg)

	-- delay before show Ready button for the player Temp --
	
	if msg.name == "showReadyButton" then
	
		UI:SendMessage("RaceJoin", {{"sgPlayShow", true }} )
	
	end

end




function not_onPlayerExit( self, msg)
		
		local player = msg.playerID
		
		
		UI:SendMessage( "pushGameState", {{"state", "gameplay" }} )
end



