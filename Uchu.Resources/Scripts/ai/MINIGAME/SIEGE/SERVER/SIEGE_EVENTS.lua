CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

function oStart(self)
   
	 GAMEOBJ:GetZoneControlID():SetActivityParams{activityID = 37, activityActive = true , modifyMaxUsers = true, maxUsers = 8, }
	
    Con = {}
    Con["GameStarted"] = false
    Con["Red_Spawners"] = 1
    Con["Blue_Spawners"] = 1
    Con["Red_Flag"] = 1   
    Con["Blue_Flag"] = 1
    Con["Red_Point"] = 1
    Con["Blue_Point"] = 1
    Con["Red_Mark"] = 1
    Con["Blue_Mark"] = 1  
    Con["Players_loaded"] = 0 
    Con["Blue_Flag_Home"] = 0
    Con["Red_Flag_Home"] = 0
    Con["Pre_Counter"] = 0
    Con["TeamRoll_1"] = "Defend"
    Con["TeamRoll_2"] = "Attack"
    Con["numofcaps"] = 0
    Con["TimeToBeat"] = 0
    Con["Round"] = 0
    Con["Round_1_Time"] = 0   
    Con["Round_2_Time"] = 0 
    
    Con['swapTeams'] = false
    Con["Round_1_Win"] = false
    Con["Round_2_Win"] = false
    
    Con["Color_Timer"] = false 
    Con["TimerStart"] = false
    Con["ReadyButton"] = false
    Con["GameStoredtime"] = self:GetVar("Set.DefendTime")
      
	for x = 1, 2 do 
		for i = 1, 4 do 
			 Con["Slot_"..i.."_Team_"..x] = "open"
		end
    end
	for i = 1, 2 do

		for x = 1, 4 do 
			

			 Con["PlayerReady_p"..x.."_t"..i] = false
			 Con["PlayerNotReady_p"..x.."_t"..i] = false
			 Con["PlayerName_p"..x.."_t"..i] = nil

		end

    end 
     
    self:SetVar("Con",Con)
    
  
    self:NotifyClientZoneObject{name = "SetRespawnTime" , param1 = self:GetVar("Set.RespawnTime") }

  
    self:SetVar("isMiniGame", "Server") 
    
    -- Set Game Parameters
    self:MiniGameSetParameters{numTeams = self:GetVar("Set.Number_Of_Teams") ,playersPerTeam = self:GetVar("Set.Number_Of_PlayersPerTeam") }

	
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
    
    -- Won Lost Timer
    
    if (msg.name == "WonLostTimer") then
    
    -- Hide Won Lost Txt
    self:NotifyClientZoneObject{ name = "ShowTxt"  ,paramStr = " "  }
    
    -- Show Score Board 
     self:NotifyClientZoneObject{ name = "ShowScoreBoardUI"  }
    
    -- Start Score Board Timer
     GAMEOBJ:GetTimer():AddTimerWithCancel(  self:GetVar("Set.ScoreBoardTimer") , "ShowRewards", self ) 
    end
    if (msg.name == "ShowRewards") then
    
    	self:NotifyClientZoneObject{ name = "SiegeRewards"  }
    	 GAMEOBJ:GetTimer():AddTimerWithCancel(  self:GetVar("Set.LeaderBoardTimer") , "ShowLeaderBoard", self ) 
    end
    
    -- Start Leader Board --
    
    if (msg.name == "ShowLeaderBoard") then
    
    	
    	sendLeaderBoardData(self)
    end
    
    
    -- Timer Player Resurrect Timer Created From the player Name
    for i = 1, self:GetVar("Set.Number_Of_Teams") do
       for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do 
             local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
             if  msg.name == player:GetName().name  then
                   player:Resurrect()
             end
       end
    end
    
    
    
     -- (2) -- 
    if (msg.name == "GateTime") then
    

        self:NotifyClientZoneObject{name = "unFreezAllPlayers", param1 = self:GetVar("Set.Number_Of_Teams") }
        self:NotifyClientZoneObject{name = "HidePlayerStats" }
        self:NotifyClientZoneObject{name = "ShowText" }
        self:NotifyClientZoneObject{name = "ShowText"}
          
        -- self:NotifyClientZoneObject{name = "Show_HelpScreen"}
        
        -- Spawn Rebuild 
         SpawnRebuilds(self)
        -- Start Gate Timer
        getObjectByName(self,"ZoneMark"):NotifyObject{name = "StartGateTimer", }
    end
    
    
    if msg.name == "EndRound"  then
    
    	self:NotifyObject{ name="EndRound", ObjIDSender = self }
    end

    
    
    if (msg.name == "StartRound2" ) then

    	 
    
    	self:MiniGameSwapTeams()
    	
    	-- teleport players to spawn points -----------------------------
		TeleportAllPlayers(self)
        -----------------------------------------------------------------
        
        removeBarrels(self)
        
        self:NotifyClientZoneObject{name = "ShowText"}
    	self:NotifyClientZoneObject{name = "Send_State", paramStr = "Round2"}
   		self:SetVar("Con.Pre_Counter" , self:GetVar("Set.Notify_Team_Objectives"))
		self:NotifyClientZoneObject{name = "HidePlayerStats" }
		self:NotifyClientZoneObject{name = "Hide_HelpScreen" }
 		self:NotifyClientZoneObject{name = "HideDropButton" }
   		self:NotifyClientZoneObject{name = "SwapTeamtxt" }
   
   		self:SetVar("Con.swapTeams", true)

   		local rebuilds = self:GetObjectsInGroup{ group = "reset" }.objects
        for i = 1, #rebuilds do 
            rebuilds[i]:RebuildReset{bFail = false}
        end
 		

 		
    	
		for i = 1, self:GetVar("Set.Number_Of_Teams") do

			   for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do 

					 local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
					 
					 local ghealth = player:GetMaxHealth{}.health
					 local gArmor = player:GetMaxArmor{}.armor
					 local gImagein = player:GetMaxImagination{}.imagination
					 

						player:SetImagination{imagination = gImagein}
						player:SetHealth{health = ghealth }
						player:SetArmor{armor = gArmor}
					
					
					 if self:MiniGameGetTeam{ playerID = player}.teamID == 1 then
						 removeflags(self,player)
						 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "reSetAnimationSet" , paramObj = player }
						 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetObjective" , param1 = 1 ,paramStr = self:GetVar("Con.TeamRoll_1") , rerouteID = player} 
						 SetPlayerRespawnLocPlayer(self, player ,1)
					 else
						 removeflags(self,player)
						 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "reSetAnimationSet" , paramObj = player }
						 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetObjective" , param1 = 2, paramStr = self:GetVar("Con.TeamRoll_2") , rerouteID = player} 
						 SetPlayerRespawnLocPlayer(self, player ,2)
					
					 end

			   end
		end
		
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "HideUI" } 
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "Show_HelpScreen" } 

		
		if self:GetVar("Con.GameStarted") then

			self:NotifyClientZoneObject{name = "Hide_HelpScreen"}
		end
    	

    	GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.Notify_Team_Objectives") , "StartRound2.1", self ) 
 		-- prep time here
		for i = 0, 4 do
			local playerOBJ = self:MiniGameGetTeamPlayers{teamID = self:MiniGameGetTeam{playerID = player }.teamID}.objects[i]
			if playerOBJ then

				if playerOBJ:GetName().name == player:GetName().name then    		
					local TID = self:MiniGameGetTeam{playerID = player }.teamID
					self:NotifyClientZoneObject{name = "SetPlayerName" ,paramStr = player:GetName().name, param1 =TID , param2 = i }
					self:NotifyClientZoneObject{name = "SetClientColor" ,paramStr = player:GetName().name, param1 =TID , param2 = i }
					
				end
			end

		end 
		UpdateScore(self)
    end
    
    if ( msg.name == "StartRound2.1" ) then
   		 self:NotifyClientZoneObject{name = "Send_State", paramStr = "StartRound2.1"}
    	-- clear Objectives and Stats -- 
    	self:NotifyClientZoneObject{name = "unFreezAllPlayers", param1 = self:GetVar("Set.Number_Of_Teams") }
    	self:NotifyClientZoneObject{name = "HidePlayerStats" }
    	self:NotifyClientZoneObject{name = "Hide_HelpScreen" }

    	
    	-- Start Round 2 Gate timer
    	getObjectByName(self,"ZoneMark"):NotifyObject{name = "StartGateTimer" }
    
    end
   

    if (msg.name == "start_count") then
    
    	
        self:NotifyClientZoneObject{name = "Send_State", paramStr = "StartGame"}
      	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5 , "StartGame", self ) 
   
          
    end
          -- (6) -- 
    if (msg.name == "StartGame") then
    
    	 -- Set State 
    	 self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox", paramStr = "Game Started"}
         self:NotifyClientZoneObject{name = "Send_State", paramStr = "StartGame"}
    	 self:NotifyClientZoneObject{name = "SendStartTimer", param1 = tonumber(0)}
    	  -- Start the RoundTimer
    	 --getObjectByName(self,"ZoneMark"):NotifyObject{name = "StartTimer"}
    	  
          -- teleport players to spawn points -----------------------------
		 TeleportAllPlayers(self)
          -------------------------------------------------------------------
          
          self:SetVar("Con.GameStarted", true)
          SpawnTeams(self)  
          self:NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Game Started"}
          self:NotifyClientZoneObject{name = "unFreezAllPlayers", param1 = self:GetVar("Set.Number_Of_Teams") }

		 -- Add Rebuilds here

          self:SetVar("Con.GameStarted", true)  

    end

    if (msg.name == "SetColor") then

			for i = 1, 2 do

				   for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do 

					     local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]

						 if self:MiniGameGetTeam{ playerID = player}.teamID == 1 then

							 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetBkGrndColor" , param1 = 1 , rerouteID = player} 
						 else

							 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetBkGrndColor" , param1 = 2 , rerouteID = player} 
						 end

				   end
			end
		self:SetVar("Color_Timer", false)
    end

 
    if (msg.name == "EndGame") then
	   
    end
  
	
end 

--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	msg.playerID:Help{ iHelpID = 2 }
	
	self:MiniGameAddPlayer{playerID = msg.playerID}
	
	if  self:GetVar("Con.TimerStart") == false then
	
		getObjectByName(self,"ZoneMark"):NotifyObject{ name = "StartJoinTimer" }
		self:SetVar("Con.TimerStart", true)
	end

	--msg.playerID:SetCollisionGroup{colGroup = 10}
	if not self:GetVar("Con.GameStarted") then
		self:NotifyClientZoneObject{name = "FreezPlayer", paramObj = msg.playerID }
	end
	msg.playerID:SetLoseCoinsOnDeath{bLoseCoinsOnDeath = false}
	
    self:NotifyClientZoneObject{name = "StoreClientPlayer" , paramObj = msg.playerID , rerouteID = msg.playerID   }
  
    self:NotifyClientZoneObject{name = "nubOfPlayers" , paramStr = self:GetVar("Set.Minimum_Players_to_Start"), rerouteID = msg.playerID   }
     
    -- update GUI with current Players
    
    for i = 1, 2 do 
    
    	for x = 1, 4 do
    		
    		if self:GetVar("Con.PlayerReady_p"..x.."_t"..i) then
    		
    		
        		self:NotifyClientZoneObject{name = "PlayerReady" , paramStr = "name_p"..x.."_t"..i..","..self:GetVar("Con.PlayerName_p"..x.."_t"..i), 
    			paramObj = msg.playerID  }	
    		
    			
				self:NotifyClientZoneObject{name = "SetPlayerName" , paramStr = self:GetVar("Con.PlayerName_p"..x.."_t"..i)  , param1 = i , param2 = x }
				self:NotifyClientZoneObject{name = "SetClientColor" ,paramStr = self:GetVar("Con.PlayerName_p"..x.."_t"..i) , param1 = i , param2 = x }
    			
 
    		end    	
    	
    	end
    	
    end
    
	if msg.playerID:IsDead{}.bDead then
		  msg.playerID:Resurrect()
	end

	--------------------------------------------
	-- Add Player to Team
	-------------------------------------------- 	 
	local TID1 = #self:MiniGameGetTeamPlayers{teamID = 1}.objects 
	local TID2 = #self:MiniGameGetTeamPlayers{teamID = 2}.objects 

	if TID1 > TID2 then

		self:MiniGameSetTeam{playerID =  msg.playerID , teamID = 2}

	elseif TID1 < TID2 then

		self:MiniGameSetTeam{playerID =  msg.playerID, teamID = 1}

	elseif TID1 == TID2 then

		self:MiniGameSetTeam{playerID =  msg.playerID, teamID = 1}

	end

	team = self:MiniGameGetTeam{ playerID =  msg.playerID}.teamID

	for i = 1, 4 do
		 print("Slot_"..i.."_Team_"..team.."     "..self:GetVar("Con.Slot_"..i.."_Team_"..team ) )
		 if self:GetVar("Con.Slot_"..i.."_Team_"..team ) == "open" then
			if team == 2 then	
				self:SetVar("Con.Slot_"..i.."_Team_"..team, "closed")
				SetTeamSlot(self,i,2,msg.playerID)				
			else
				self:SetVar("Con.Slot_"..i.."_Team_"..team, "closed")
				SetTeamSlot(self,i,1,msg.playerID)
			end
			break
		end
	end
	
	if  self:GetVar("Con.GameStarted") then
			self:NotifyClientZoneObject{name = "HideQueUI" } 
			self:NotifyClientZoneObject{name = "ShowHUDUI" }
				 
	end
	self:AddActivityUser{ userID = msg.playerID }
	--self:RequestActivityEnter{bStart = true, userID = msg.playerID}
	GetLeaderboardData( self,  msg.playerID, 37 )
   
end

function SetTeamSlot(self,x,i,player)


		
		self:NotifyClientZoneObject{name = "JoinTeam" , paramStr = "name_p"..x.."_t"..i , paramObj = player }
		self:NotifyClientZoneObject{name = "EnableCheck" , paramStr = "name_p"..x.."_t"..i , paramObj = player , rerouteID = player} 
		self:NotifyClientZoneObject{name = "ReadyTeam" , paramStr = "ready_p"..x.."_t"..i , paramObj = player }
		self:SetVar("Con.PlayerReady_p"..x.."_t"..i, true)
		self:SetVar("Con.PlayerName_p"..x.."_t"..i, player:GetName().name)	

		--------------------------------------------
		-- Sets Player Respan Locations
		--------------------------------------------  
		SetPlayerRespawnLoc(self, player)

		setPlayerScores(self, player)

		-- Send MSG CLIENT             
		self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_2") }
		-- Set Gui Color
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetBkGrndColor" , param1 = i , rerouteID = player } 


		self:SetVar("Con.Players_loaded" ,self:GetVar("Con.Players_loaded") + 1)

		TeleportPlayer(self,player)

		self:NotifyClientZoneObject{name = "FreezPlayer", paramObj = player }

		player:SetPVPStatus{ bOn = true }

		if not self:GetVar("Con.ReadyButton") then
			RoutToPlayer{msg, name = "ShowPlayButton", paramObj = player, playerID = player }
			self:SetVar("Con.ReadyButton" , true)	

		end
		self:NotifyClientZoneObject{name = "SetPlayerName" ,paramStr = player:GetName().name, param1 = i , param2 = x }		
		


				



		

		
end




function onPlayerExit( self, msg)
		 local player = msg.playerID
		 
		 GAMEOBJ:GetZoneControlID():RemoveActivityUser{ userID = player } 
		--- Refresh GUI
		self:SetVar("Con.Players_loaded", self:GetVar("Con.Players_loaded") - 1) 
		 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "ClearUI", rerouteID = player} 
      
        self:MiniGameRemovePlayer{ playerID= player}
        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_3")}
        
        for i = 1, 2 do
        
        	for x = 1, 4 do
        	
        		if self:GetVar("Con.PlayerName_p"..x.."_t"..i) ==  player:GetName().name then
        		
        		
        			self:NotifyClientZoneObject{name = "removeplayer" , paramStr = "p"..x.."_t"..i , paramObj = player }
        			   		
					self:SetVar("Con.PlayerReady_p"..x.."_t"..i, nil)
					self:SetVar("Con.PlayerNotReady_p"..x.."_t"..i, nil)
					self:SetVar("Con.PlayerName_p"..x.."_t"..i, nil)
        			self:SetVar("Con.Slot_"..x.."_Team_"..i,"open" ) 
        			break
        		end
        	
        	
        	end
        
        end
end


function onActivityStateChangeRequest(self,msg)
	

	

		if (msg.wsStringValue == "gamestate" ) then 
		   self:NotifyClientZoneObject{name = "SetGameState" , paramStr = "Siege"  }
		
		end

		
	

end


function mainChildLoaded(self, msg) 
    local obj = msg.childID
    if msg.templateID == self:GetVar("Set.QB_Object_LOT") then
    
        for i = 1, 3 do
            if self:GetVar("Barrel_"..i) == nil then
                storeObjectByName(self, "Barrel_"..i, obj)
                break
            end
        end
    end
end
--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)	

    local player = msg.sender
    
	if (msg.iButton == 1 and msg.identifier == "Drop") then

		if self:MiniGameGetTeam{ playerID = player}.teamID == 2 then
			 local FlagFound = removeflags(self, player)

			 if FlagFound then
			 	  player:SetSkillRunSpeed{Modifier = 500 }
			 	   
                  DoObjectAction(player, "stopeffects", "godlight")
				  self:NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = player:GetName().name..self:GetVar("Set.Info_Text_5") }
				  local bluepos =   player:GetPosition().pos 
				  local config = { {"AtHomePoint", false }, {"taken_name", FlagFound}   }
				  RESMGR:LoadObject { owner = self, objectTemplate = self:GetVar("Set.QB_Object_LOT") , x= bluepos.x, y= bluepos.y , z= bluepos.z, configData = config  }        
			end
		end

	end
	
	
	
	   
		
		
	
	   
	   
	   if (msg.iButton == 2469) then
	   
	   	
	   		
	   		local newString = string.sub(msg.identifier, 7)
			self:SetVar("Con.PlayerReady_p"..newString, nil)
			self:SetVar("Con.PlayerNotReady_p"..newString, nil)
			self:SetVar("Con.PlayerName_p"..newString, nil)
	   		
	   
	   end
		if ( msg.identifier == "playSiege") then

				self:ActivityStart{}
				self:SetVar("Con.GameStarted",true) 

				 self:NotifyClientZoneObject{name = "unFreezAllPlayers", param1 = self:GetVar("Set.Number_Of_Teams") }
			
				--------------------------------------------
				-- Start Game if we have our players
				--------------------------------------------    
				
			
				
				
				if self:GetVar("Con.Players_loaded") >= self:GetVar("Set.Minimum_Players_to_Start") or player:GetGMLevel{}.bIsGM  then
					
					  GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.Notify_Team_Objectives") , "GateTime", self ) 
					  self:NotifyClientZoneObject{name = "resetCaptured"} 

					  self:SetVar("Con.Pre_Counter",self:GetVar("Set.Notify_Team_Objectives")  )
					  self:SetVar("TimeToBeat" , self:GetVar("Set.DefendTime") )

					  self:NotifyClientZoneObject{name = "TimeToBeat" ,paramStr = self:GetVar("TimeToBeat")  }
					  self:NotifyClientZoneObject{name = "Send_State", paramStr = "Game Pre Start"}

					 for i = 1, 2 do
					 
						for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
							 local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
							 print(player:GetName().name.."    "..self:GetVar("Con.TeamRoll_"..i) )
							 if self:MiniGameGetTeam{ playerID = player}.teamID == 1 then
								 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetObjective" , paramStr = self:GetVar("Con.TeamRoll_1") , rerouteID = player} 
								 GAMEOBJ:GetZoneControlID():RequestActivityStartStop{bStart = true, userID = player}
							 else
								 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "UISetObjective" , paramStr = self:GetVar("Con.TeamRoll_2") , rerouteID = player} 
								  GAMEOBJ:GetZoneControlID():RequestActivityStartStop{bStart = true, userID = player}
							 end

						end
					end

					

				   self:NotifyClientZoneObject{name = "HideQueUI" } 
				   self:NotifyClientZoneObject{name = "Show_HelpScreen" } 
				end 
			end
end
-- Notify messages from Activity Manager
----------------------------------------------------------------
-- notify from activity mng: When activity is stopped this is 
-- needed to update the leaderboard.
----------------------------------------------------------------
function onDoCalculateActivityRating(self,other,msg)
   
    msg.outActivityRating = msg.fValue1
 
    return msg
end



--------------------------------------------------------------
-- User Exits Activity
--------------------------------------------------------------

function onRequestActivityExit(self, msg)
    if (msg.bUserCancel) == true then
        -- forward this request on to the zone control object
      
        GAMEOBJ:GetZoneControlID():RequestActivityExit{bUserCancel = msg.bUserCancel, userID = msg.userID}
	end
end