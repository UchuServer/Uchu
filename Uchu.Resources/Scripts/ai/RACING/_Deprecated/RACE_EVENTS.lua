CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

function oStart(self)
      
	---------------------------------------------------------------------------------------
	--   Set Activity Params ( activityID, max Players ) 
	---------------------------------------------------------------------------------------
    self:SetActivityParams{ activityID = self:GetVar("Set.activityID") ,  
    modifyMaxUsers = true, maxUsers = self:GetVar("Set.Number_Of_PlayersPerTeam"), 
    modifyActivityActive = true,  
    activityActive = true};
    
	---------------------------------------------------------------------------------------
	--   Global Vars onStart
	---------------------------------------------------------------------------------------    
    
    Con = {}
    Con["GameStarted"] = false
    Con["Blue_Mark"] = 1  
    Con["Players_loaded"] = 0 
 
	for i = 1, 6 do 
	
		Con["Slot_"..i.."_Team_1"] = "open"
		Con["Start_Pos_"..i] = "open"
		
		-- *Temp Loading GUI Start* --
		Con["PlayerReady_p"..i.."_t1"] = false
		Con["PlayerNotReady_p"..i.."_t1"] = false
		Con["PlayerName_p"..i.."_t1"] = nil
		-- *Temp Loading GUI End* --
	end
  
    self:SetVar("Con",Con)
	
end

--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)
	--------------------------------------------------------------
	-- Set GUI State to Racing *Client*
	--------------------------------------------------------------
    self:NotifyClientZoneObject{ name = "pushStateRace" , rerouteID = msg.playerID  }	
 
	--------------------------------------------------------------
	-- Sets Client Activity Params for the Player loading
	--------------------------------------------------------------	
    self:NotifyClientZoneObject{name = "SetActivityParams" , param1 = self:GetVar("Set.activityID") , rerouteID = msg.playerID   }
	--------------------------------------------------------------
	-- Create Global Values Related to the Player  
	--------------------------------------------------------------	 
    self:SetVar(msg.playerID:GetName().name,nil) 
	self:SetVar("LastNode_"..msg.playerID:GetName().name , 0 )
    self:SetVar("ValidLap_"..msg.playerID:GetName().name, false)
        
	--------------------------------------------------------------
	-- Sets Flag to show the minimap
	--------------------------------------------------------------
	msg.playerID:Help{ iHelpID = 2 }
	
	GAMEOBJ:AddObjectToAlwaysInScopeList( msg.playerID )

	--------------------------------------------------------------
	-- Create / Set Varables to the GUI and Client Zone Script
	--------------------------------------------------------------	
	-- Store Player on the Client Needed for GUI
    self:NotifyClientZoneObject{name = "StoreClientPlayer" , paramObj = msg.playerID , rerouteID = msg.playerID   }
    
  	-- Set The minimum number of player to start *Client*
    self:NotifyClientZoneObject{name = "nubOfPlayers" , paramStr = self:GetVar("Set.Minimum_Players_to_Start"), rerouteID = msg.playerID   }
    
    -- FreezePlayer *Client*
    self:NotifyClientZoneObject{name = "FreezPlayer" , paramObj = msg.playerID , rerouteID = msg.playerID   } 
     
  

	--------------------------------------------------------------
	-- Check to see if Player is Alive if not Resurrect 
	--------------------------------------------------------------	   
	if msg.playerID:IsDead{}.bDead then
		  msg.playerID:Resurrect()
	end

	--------------------------------------------------------------
	-- Finds an Open Slot to Teleport Player 
	--------------------------------------------------------------	
	-- *Temp Loading GUI Start* --	
	for i = 1, 6 do
		 if self:GetVar("Con.Slot_"..i.."_Team_1" ) == "open" then
			self:SetVar("Con.Slot_"..i.."_Team_1", "closed")
			SetTeamSlot(self,i,1,msg.playerID)
			break
		end
	end
	-- *Temp Loading GUI End* --
	

	--------------------------------------------------------------
	-- Teleport Player to His/Her Car Pos
	--------------------------------------------------------------	
	TeleportPlayer(self,msg.playerID)
	
	--------------------------------------------------------------
	-- get the leaderboard data for the user 
	--------------------------------------------------------------	    
	GetLeaderboardData( self,  msg.playerID, self:GetVar("Set.activityID") )

	-- *Temp Loading GUI Start* --
	--------------------------------------------------------------
	-- Start 3 second timer ( show the Place holder Matching GUI ) 
	--------------------------------------------------------------	
	GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , msg.playerID:GetName().name.."bePlayerReady", self )
    -- *Temp Loading GUI End* --    

    -- Set Path name on the *Client*
    self:NotifyClientZoneObject{name = "SetPath", paramStr = self:GetVar("Set.Race_PathName"), rerouteID = msg.playerID }
  
	--------------------------------------------------------------
	-- 1st Player to enter Sets the Path Name on the Server 
	--------------------------------------------------------------	
	
  	if  self:GetVar("Set.PathName") == nil then
  	
		self:SetCurrentPath{pathName = self:GetVar("Set.Race_PathName") }
		self:SetVar("SetPathName", true) 
		
	end      

   
end

-- *Temp Loading GUI Start* --	
function SetTeamSlot(self,x,i,player)

		self:NotifyClientZoneObject{name = "JoinTeam" , paramStr = "name_p"..x.."_t"..i , paramObj = player }
		self:NotifyClientZoneObject{name = "EnableCheck" , paramStr = "name_p"..x.."_t"..i , paramObj = player , rerouteID = player} 
	
		self:SetVar("Con.PlayerReady_p"..x.."_t"..i, true)
		self:SetVar("Con.PlayerName_p"..x.."_t"..i, player:GetName().name)	

		self:SetVar("Con.Players_loaded" ,self:GetVar("Con.Players_loaded") + 1)

		if not self:GetVar("Con.ReadyButton") then
			RoutToPlayer{msg, name = "ShowPlayButton", paramObj = player, playerID = player }
			self:SetVar("Con.ReadyButton" , true)	
		end
		self:NotifyClientZoneObject{name = "SetPlayerName" ,paramStr = player:GetName().name, param1 = i , param2 = x }		
			
end
-- *Temp Loading GUI End* --	


function onPlayerExit( self, msg)
		
		local player = msg.playerID

		GAMEOBJ:RemoveObjectFromAlwaysInScopeList( player )
				
		GAMEOBJ:GetZoneControlID():RemoveActivityUser{ userID = player } 

		--- Clear GUI on Player Exit
		self:SetVar("Con.Players_loaded", self:GetVar("Con.Players_loaded") - 1) 
		
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "ClearUI", rerouteID = player} 
      
		-- *Temp Loading GUI Start* --	         
        for i = 1, 2 do
        
        	for x = 1, 6 do
        	
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
        -- *Temp Loading GUI End* --	
end

--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)	

    local player = msg.sender
    ----------------------------------------------------------
    -- Reward Exit Button
    ----------------------------------------------------------    
    if ( msg.identifier  == "buttonExit" ) then
     	self:NotifyClientZoneObject{name = "exit" , rerouteID = player  }
    
    end
    ----------------------------------------------------------
    -- Reward Replay Button
    ----------------------------------------------------------    
    if ( msg.identifier == "buttonReplay")  then
    
     	self:NotifyClientZoneObject{name = "replay" , rerouteID = player  }
    
    end   
    ----------------------------------------------------------
    -- Reward Button
    ----------------------------------------------------------
    if ( msg.identifier == "rewardButton" ) then
    
	    --// Show LeaderBoard GUI //--
	    self:RequestActivitySummaryLeaderboardData{ user = player, queryType = 7 } 
	    
	    --// Show Reward GUI //--
	    self:NotifyClientZoneObject{name = "rewardGUI" , rerouteID = player  }
	     
    end
     
    ----------------------------------------------------------
    -- Play Race Button  -- *Temp* GUI
    ----------------------------------------------------------
	if ( msg.identifier == "playRace") then
	   
		-- Tell Client To Start Race
		self:NotifyClientZoneObject{name = "StartRace" }
		
		------------------------------------------------------
		-- Players Posses Cars
		-- Start Count Down
		-- Notify Client to Start Activity
		-- Notify Server to Start Activity
		-- Notify Players client of the Path Name
		-- Start Count Down
		------------------------------------------------- -----
		local objects = self:GetAllActivityUsers{}.objects

		for i = 1, #objects do
			
			local player =  objects[i]
			local Child  =   getObjectByName(self, player:GetName().name.."CarID") 	
	    	player:PrepareForPossession{ objToPossess = Child }
			self:NotifyClientZoneObject{ name = "StartCountDown"  } 
			self:NotifyClientZoneObject{name = "ActivityStart" , rerouteID = player  }
			self:ActivityStart{rerouteID = player}
			
			
		end
		
			-- Start Race Timer here
	 		GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "Start_Race", self )
	end
	
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

function onChildLoaded(self, msg)


	if (msg.childID:GetType().objType == "Vehicle") then 
	

		local Child = msg.childID
        local Driver = getObjectByName(Child, "RaceDriver")
        local SpawnMark =  Child:GetVar("SpawnMark")   
		local StartPos = Child:GetVar("StartPos")
		local CarColor = Child:GetVar("CarColor")
		

		-- Store Players Car on the Server
		storeObjectByName(self, Driver:GetName().name.."CarID", Child)
		
		-- Store Car object on the Client
		self:NotifyClientZoneObject{ name = "storePlayersCarID" , paramObj = Child , paramStr = Driver:GetName().name.."CarID" }
		
 		---------------------------------------------------------------
		-- Add Player to Activity
		---------------------------------------------------------------
		 AddPlayerAndCarToActivity(self,Driver,Child )
		 
		setPlayerScores(self, Driver) 
		
        self:NotifyVehicleOfRacingObject{racingObjectID = Child }
        
		self:NotifyClientZoneObject{ name = "SetDriverPos" , param1 = StartPos, paramObj = Child , paramStr = Driver:GetName().name }
		 
		self:RacingPlayerRankChanged{playerID = Driver, oldRank = Child:GetVar("StartPos") - 1, newRank = Child:GetVar("StartPos") }

    end
    
end
---------------------------------------------------------------
-- Main Server Zone Script onTimer Functions
---------------------------------------------------------------	
onTimerDone = function(self, msg)
	local objects = self:GetAllActivityUsers{}.objects
	
	for x = 1, #objects do
			
			local player = objects[x]
			if (player) then
			
				if msg.name == player:GetName().name.."bePlayerReady" then
					self:NotifyClientZoneObject{name = "reSetRaceGUI" , paramStr = tostring(self:GetVar("Set.Number_of_Laps")), rerouteID = player  }
					bePlayerReady(self)
					
				end
			end
	end

	--- unlock player cars
	if msg.name == "Start_Race" then
		
			for i = 1, #objects do

				local player =  objects[i]
				local Child  =   getObjectByName(self, player:GetName().name.."CarID") 
				Child:VehicleUnlockInput{bLockWheels = true}

				ServerActivityTimers(self, player ,"StartTotalTime" , false)
				
				self:NotifyClientZoneObject{name = "ActivityTimers" , paramObj = player , paramStr = "StartTotalTime,false",  rerouteID = player  }
				self:SetVar(player:GetName().name,"running")

			end
			
	end
	-- update other Driver that have finished
	if msg.name == "Fline" then
		
			
			for i = 1, #objects do
			
				local player = objects[i]
				
				
					if self:GetVar(player:GetName().name.."_Stats") == "Finished"  then
					
						local playerName = player:GetName().name
	 					local pos =   self:GetVar( playerName.."_finalPos")
	 					local BTime = self:GetVar( playerName.."_BestTime")
	 					local TTime = self:GetVar( playerName.."_TotalTimer")
                     
	
						self:NotifyClientZoneObject{ name = "place_"..pos , paramStr = ""..playerName..","..BTime..","..TTime.."" , paramObj = player }						
	
					end
					
	
		    end
	
	end
	
	
end


-- *Temp Loading GUI Start* --	
function bePlayerReady(self)

		local objects = self:GetAllActivityUsers{}.objects
		
		for x = 1, #objects do
		
			local player = objects[x]
			
				if self:GetVar("Con.PlayerReady_p"..x.."_t1") then
					if ( self:GetVar("Con.PlayerName_p"..x.."_t1") == player:GetName().name ) then

					self:NotifyClientZoneObject{name = "ShowPlayButton" , paramObj = player, playerID = player, rerouteID = player  }
					end
					self:NotifyClientZoneObject{name = "PlayerReady" , paramStr = "name_p"..x.."_t1"..","..self:GetVar("Con.PlayerName_p"..x.."_t1"), 
					paramObj = player }	
					self:NotifyClientZoneObject{name = "SetPlayerName" , paramStr = self:GetVar("Con.PlayerName_p"..x.."_t1")  , param1 = i , param2 = x }

				end
				
				updateAllStartPos(self)

		end
		
end
-- *Temp Loading GUI End* --	


--------------------------------------------------------------
-- Updates all Starting Driver Pos/Flags
--------------------------------------------------------------
function updateAllStartPos(self)

		local objects = self:GetAllActivityUsers{}.objects
		
		for x = 1, #objects do
		
			local player = objects[x]
			local carID = getObjectByName(self, player:GetName().name.."CarID")
			-- update top left hud with Pos
			local StartPos = carID:GetVar("StartPos")
			self:NotifyClientZoneObject{ name = "SetDriverPos" , param1 = StartPos, paramObj = player , paramStr = player:GetName().name }
			
			carID:NotifyObject{ name = "updateCarNumber" , param1 = StartPos }
			
		end

end
--------------------------------------------------------------
-- Zone on Notify Objects
--------------------------------------------------------------
function onNotifyObject(self, msg)
	

	if msg.name == "UpdateScore" then
		UpdateScore(self)
	-- Store Start pos IDs
    elseif ( msg.name == "Blue_Mark" ) then
        storeObjectByName(self, "Blue_Mark_"..msg.ObjIDSender:GetVar("placement"), msg.ObjIDSender)     
   
	end 
	
	
end