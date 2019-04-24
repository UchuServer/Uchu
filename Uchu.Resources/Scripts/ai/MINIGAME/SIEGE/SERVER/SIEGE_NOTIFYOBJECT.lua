-------------------------------------------------------------

function mainNotifyObject(self, msg)
	-- up Keep var Barrels
	if msg.name == "removeBarrel" then
		for i = 1, 3 do
            if getObjectByName(self,  "Barrel_"..i ) then
                if (msg.ObjIDSender:GetID() == getObjectByName(self,  "Barrel_"..i ):GetID()) then
                    self:SetVar("Barrel_"..i, nil) 
                end
            end
         end
	end

	if msg.name == "UpdateScore" then
	
		UpdateScore(self)
	
	end
	
	-- player Smashed A Smashable
	if msg.name == "reBuild" then
		print("reBuild "..msg.ObjIDSender:GetName().name)
		self:MiniGameAddPlayerScore{playerID =  msg.ObjIDSender, scoreType = 7, score = 1 } -- rebuilds
		UpdateScore(self)
		
	end
	if msg.name == "SmashQB" then
		print("SmashQB "..msg.ObjIDSender:GetName().name)
		self:MiniGameAddPlayerScore{playerID = msg.ObjIDSender , scoreType = 8, score = 1 } -- rebuilds
		UpdateScore(self)
		
	end	
	
	----------------------------------------------------------------------------
	----------------------------------------------------------------------------
	-- GAME TIMMER ROUNDS 
	----------------------------------------------------------------------------
	----------------------------------------------------------------------------

	if msg.name == "timeExpired" then
	
			self:SetVar("Con.Round", self:GetVar("Con.Round") + 1)
			timetobeat = self:GetVar("Set.DefendTime")
			getObjectByName(self,"ZoneMark"):NotifyObject{name = "StopTimer"}
		
		  if self:GetVar("Con.Round") == 1  then
            removeBarrels(self)
            ----------------------------------------------------------------------------
            -- Round 1 timer expire 
            ----------------------------------------------------------------------------
            
			--self:NotifyClientZoneObject{name = "EndRoundTxt" , paramStr = "Timer is Out 1" }
			RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr = "Time to Beat set to "..SecondsToClock(timetobeat) ,paramObj = nil, team = 1 }
			
			if self:GetVar("Con.Round_1_Win") == true then
				RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr = "Captured in "..SecondsToClock(timetobeat)  ,paramObj = nil, team = 2 }
			else
				RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr = "Time remains "..SecondsToClock(timetobeat)  ,paramObj = nil, team = 2 }
			end
			
			
			GAMEOBJ:GetTimer():AddTimerWithCancel(  2 , "EndRound", self ) 



		  end
			  
		   if self:GetVar("Con.Round") == 2  then

            ----------------------------------------------------------------------------
            -- Timer expire Round 2 
            ----------------------------------------------------------------------------
			   		--self:NotifyClientZoneObject{name = "EndRoundTxt" , paramStr = "Timer expire Round 2 " }
			   		
					
				
					if self:GetVar("Con.Round_1_Time") <  self:GetVar("Con.Round_2_Time") then

						RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_11")  ,paramObj = nil, team = 2 }
						RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_10")   ,paramObj = nil, team = 1 }

					elseif self:GetVar("Con.Round_1_Time") >  self:GetVar("Con.Round_2_Time") then

						RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_10")  ,paramObj = nil, team = 2 }
						RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_11")   ,paramObj = nil, team = 1 }

					elseif self:GetVar("Con.Round_1_Time") ==  self:GetVar("Con.Round_2_Time") then

						self:NotifyClientZoneObject{ name = "EndRoundTxt"  ,paramStr = "TIE! "..self:GetVar("Con.Round_1_Time").." To "..self:GetVar("Con.Round_2_Time")  }
					end
					
					
				--Start timer  Won Lost
				 self:NotifyClientZoneObject{name = "FreezAllPlayers" , param1 =  self:GetVar("Set.Number_Of_Teams") }
			   	 GAMEOBJ:GetTimer():AddTimerWithCancel(  self:GetVar("Set.WonLoastMatchTimer") , "WonLostTimer", self ) 

		   end
			

	
	end



	----------------------------------------------------------------------------
	----------------------------------------------------------------------------
	-- GAME BARREL CAPTS  
	----------------------------------------------------------------------------
	----------------------------------------------------------------------------
	if  msg.name == "Captured_Object"  then
	
		self:SetVar("Con.numofcaps", self:GetVar("Con.numofcaps" ) + 1)
		local numOfCaps =  self:GetVar("Con.numofcaps")
	
		if numOfCaps <= 3 then	
			self:NotifyClientZoneObject{name = "SendCaptured", param1 = numOfCaps}
		end
		
		if numOfCaps == 3 then
			self:SetVar("Con.Round", self:GetVar("Con.Round") + 1)
		    local timetobeat = self:GetVar("CurrentTime")
		    local timetobeatIn = self:GetVar("Con.GameStoredtime") - self:GetVar("CurrentTime") 
		    
			if self:GetVar("Con.Round") == 1 then
				self:SetVar("Con.Round_1_Time", timetobeat)
				self:SetVar("Con.Round_1_Win", true) 
			else
				self:SetVar("Con.Round_2_Time", timetobeat + self:GetVar("Con.Round_1_Time") )
				self:SetVar("Con.Round_2_Win", true) 
			end
		
			self:SetVar("Con.numofcaps",0)

			getObjectByName(self,"ZoneMark"):NotifyObject{name = "StopTimer"}
			
			GAMEOBJ:GetZoneControlID():SetVar("Set.DefendTime", timetobeatIn )
			
			  
			  if self:GetVar("Con.Round") == 1  then

            ----------------------------------------------------------------------------
            -- Capted 3 Barrels Round 1 
            ----------------------------------------------------------------------------
				--self:NotifyClientZoneObject{name = "EndRoundTxt" , paramStr = "Capted All Three Barrels Round 1" }

                self:NotifyClientZoneObject{ name = "EndRoundTxt"  ,paramStr = "Attacking Team Captured All Barrels in: "..SecondsToClock(timetobeatIn) }
                
                self:NotifyClientZoneObject{name = "TimeToBeat" ,paramStr = timetobeatIn  }
			  	GAMEOBJ:GetTimer():AddTimerWithCancel(  2 , "EndRound", self ) 
			  
			  
			  
			 elseif self:GetVar("Con.Round") == 2  then	
            ----------------------------------------------------------------------------
            -- Capted 3 Barrels Round 2
            ----------------------------------------------------------------------------
			-- self:NotifyClientZoneObject{name = "EndRoundTxt", paramStr = "Capted All Three Round 2" }
			   	
			
			   		
			   		if self:GetVar("Con.Round_1_Time") <  self:GetVar("Con.Round_2_Time") then
			   		 --self:NotifyClientZoneObject{ name = "EndRoundTxt"  ,paramStr = self:GetVar("Set.Info_Text_9")  }
					RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_8")  ,paramObj = nil, team = 1 }
					RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_9")   ,paramObj = nil, team = 2 }
			   		
			   		
			   		elseif self:GetVar("Con.Round_1_Time") >  self:GetVar("Con.Round_2_Time") then
			   		 --self:NotifyClientZoneObject{ name = "EndRoundTxt"  ,paramStr = self:GetVar("Set.Info_Text_8")   }
					RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_8")  ,paramObj = nil, team = 2 }
					RoutToTeam{ name = "EndRoundTxt" , param1 = nil, param2 = nil ,paramStr =  self:GetVar("Set.Info_Text_9")   ,paramObj = nil, team = 1 }

			   		elseif self:GetVar("Con.Round_1_Time") ==  self:GetVar("Con.Round_2_Time") then
			   		
			   			self:NotifyClientZoneObject{ name = "EndRoundTxt"  ,paramStr = "TIE! "..self:GetVar("Con.Round_1_Time").." To "..self:GetVar("Con.Round_2_Time")  }


			   		end
			   		
			   	 self:NotifyClientZoneObject{name = "FreezAllPlayers" , param1 =  self:GetVar("Set.Number_Of_Teams") }
			   	 --Start timer  Won Lost
			   	 GAMEOBJ:GetTimer():AddTimerWithCancel(  self:GetVar("Set.WonLoastMatchTimer") , "WonLostTimer", self ) 
			   	 
			   	 
			   
			   end
			
		 end
		
		
	
	end



	if  msg.name == "GameOver"  then
		--self:NotifyClientZoneObject{name = "FreezAllPlayers" , param1 =  self:GetVar("Set.Number_Of_Teams") }
		self:NotifyClientZoneObject{name = "Send_State", paramStr = "GameOver"}
	end


   -- (4) -- 		
   if  msg.name == "EndGateTime"  then
   		self:NotifyClientZoneObject{name = "Send_State", paramStr = "Gate Opend"}
   		
   		getObjectByName(self,"ZoneMark"):NotifyObject{name = "StartTimer"}
		
   end
  
  
   if  msg.name == "EndRound" then
   
   		-- Respawn Cap Objects
   	
   
   		-- Lock Players
   		self:NotifyClientZoneObject{name = "Send_State", paramStr = "RoundEnd"}
   		self:NotifyClientZoneObject{name = "FreezAllPlayers" , param1 =  self:GetVar("Set.Number_Of_Teams") }
   		-- Open All Player Stats
   		self:NotifyClientZoneObject{name = "ShowPlayerStats" }
   		-- Timer Before unlocking
   		
   		
   		
   		GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("Set.State_Results_Timer") , "StartRound2", self ) 
   			-- Close Gate
      		for i = 1, #self:GetObjectsInGroup{ group = "gate"}.objects do
      		    local gate = self:GetObjectsInGroup{ group = "gate"}.objects[i]
      		    gate:GoToWaypoint{iPathIndex = 0}
   			end
   end
  




    if ( msg.name == "Blue_Spawn" ) then
        storeObjectByName(self, "Blue_Spawn_"..self:GetVar("Con.Blue_Spawners"), msg.ObjIDSender)
        local i =  self:GetVar("Con.Blue_Spawners") + 1
        self:SetVar("Con.Blue_Spawners", i ) 
		msg.ObjIDSender:SetVar("TeamID", 1)
    end
    if ( msg.name == "Red_Spawn" ) then
        storeObjectByName(self, "Red_Spawn_"..self:GetVar("Con.Red_Spawners"), msg.ObjIDSender)
        local i = self:GetVar("Con.Red_Spawners") + 1
        self:SetVar("Con.Red_Spawners", i ) 
		msg.ObjIDSender:SetVar("TeamID", 2)

    end

    
    
    if ( msg.name == "Blue_Flag" ) then
        storeObjectByName(self, "Blue_Flag_"..self:GetVar("Con.Blue_Flag"), msg.ObjIDSender)
        local i =  self:GetVar("Con.Blue_Flag") + 1
        self:SetVar("Con.Blue_Flag", i )    
    end
    if ( msg.name == "Red_Flag" ) then
        storeObjectByName(self, "Red_Flag_"..self:GetVar("Con.Red_Flag"), msg.ObjIDSender)
        local i = self:GetVar("Con.Red_Flag") + 1
        self:SetVar("Con.Red_Flag", i )      
    end
    if ( msg.name == "Red_Point" ) then
        storeObjectByName(self, "Red_Point_"..self:GetVar("Con.Red_Point"), msg.ObjIDSender)
        local i = self:GetVar("Con.Red_Point") + 1
        self:SetVar("Con.Red_Point", i )      
    end
    if ( msg.name == "Red_Mark" ) then
        storeObjectByName(self, "Red_Mark_"..self:GetVar("Con.Red_Mark"), msg.ObjIDSender)
        local i = self:GetVar("Con.Red_Mark") + 1
        self:SetVar("Con.Red_Mark", i )     
    end
    if ( msg.name == "Blue_Point" ) then
    	
        storeObjectByName(self, "Blue_Point_"..self:GetVar("Con.Blue_Point"), msg.ObjIDSender)
        local i = self:GetVar("Con.Blue_Point") + 1
        self:SetVar("Con.Blue_Point", i )      
    end
    if ( msg.name == "Blue_Mark" ) then
        storeObjectByName(self, "Blue_Mark_"..self:GetVar("Con.Blue_Mark"), msg.ObjIDSender)
        local i = self:GetVar("Con.Blue_Mark") + 1
        self:SetVar("Con.Blue_Mark", i )      
    end
    
    -- Seige timer is set on this object
     if ( msg.name == "Zone_Mark" ) then
         storeObjectByName(self, "ZoneMark", msg.ObjIDSender)      
    end
    

    
      
end





