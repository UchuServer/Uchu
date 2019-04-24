 --------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
function onStartup(self)
    Con = {}
    Con["Red_Spawners"] = 1
    Con["Blue_Spawners"] = 1
    Con["rSpawn"] = 0 
    self:SetVar("Con",Con)
	UI:SendMessage( "pushGameState", {{"state", "Siege" }} )
	UI:SendMessage("ShowUI", { {"show", true } })
	

	PHYSICS:SetCanCollide(10, 10, true)


end

function onNotifyClientZoneObject(self,msg) 
                --these pushes are questionable, is this a workaround from the old system?
	UI:SendMessage( "pushGameState", {{"state", "Siege" }} )
	if msg.name == "ShowText" then
		UI:SendMessage("SiegeBigTxt", {{"bigtxtVisible", "hide" }} )
		UI:SendMessage("SiegeText", {{"Text", " " }} )
		UI:SendMessage("SiegeText", {{"UI", "show" }} )
	elseif msg.name == "SetGameState" then	
		UI:SendMessage( "pushGameState", {{"state", "Siege" }} )
	elseif msg.name == "nubOfPlayers" then	
		UI:SendMessage("SiegeJoin", {{"nubOfPlayers", msg.paramStr} })
	elseif msg.name == "HideUI" then
		UI:SendMessage("SiegeUI", { {"UI", "hide"} })
	elseif msg.name == "HideText" then
		UI:SendMessage("SiegeText", {{"UI", "hide" }} )
	elseif msg.name == "OverHeadText" then
		UI:SendMessage("SiegeText", {{"Text", msg.paramStr }} )
	elseif msg.name == "ShowPlayButton" then
		UI:SendMessage("SiegeJoin", {{"sgPlayShow", true }} )

		
	elseif msg.name == "PlayerReady" then
		UI:SendMessage("SiegeJoin", {{"playerready", msg.paramStr }} )
		
	elseif msg.name == "removeplayer" then
		UI:SendMessage("SiegeJoin", {{"removeplayer", msg.paramStr }} )
	elseif msg.name == "EnableCheck" then
		for i = 1, 2 do
			for x = 1, 4 do 	
				if msg.paramStr == "name_p"..x.."_t"..i then		
					UI:SendMessage("SiegeJoin", {{"EnableCheck", msg.paramStr }} )			
				end	
			end	
		end	
	elseif msg.name == "JoinTeam" then
		for i = 1, 2 do
			for x = 1, 4 do 
				if msg.paramStr == "name_p"..x.."_t"..i then		
					UI:SendMessage("SiegeJoin", {{"join_name", msg.paramStr },{ "playername", msg.paramObj:GetName().name }} )
				
				end	
			end
		end
	elseif msg.name == "ReadyTeam" then
		for i = 1, 2 do
			for x = 1, 4 do 
				if msg.paramStr == "ready_p"..x.."_t"..i then
					UI:SendMessage("SiegeJoin", {{"join_ready", msg.paramStr }} )
				end	
			end
		end
	elseif msg.name == "PlayerLoaded" then
			 for i = 1, 2 do
					 for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
						 local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
						 local TeamColor = self:MiniGameGetTeamColor{teamID = i}.color
						if GAMEOBJ:GetZoneControlID():MiniGameGetTeam{ playerID = player}.teamID == 1 then
							 colors = 1
						 else
							 colors = 0
						 end             
						 player:SwapColor{ bodyPiece = 1 , color = colors }  -- Set Chest Color 
						 player:SwapColor{ bodyPiece = 2 , color = colors }  -- Set Legs Color     
					 end
			end
	elseif msg.name == "ClearUI" then
	
		--UI:SendMessage("ToggleActivityCloseButton", {{"Toggle", true }} )
	
		UI:SendMessage("SiegeRewards", {{"rewardsVisible", "hide" }} )
		UI:SendMessage("SiegeBigTxt", {{"bigtxtVisible", "hide" }} )
		UI:SendMessage("SiegeUI", {{"clearNames", true}} )
		UI:SendMessage("SiegeJoin", {{"clearGUI", true }} )
		UI:SendMessage("SiegeJoin", {{"UI", "show" }} )
		UI:SendMessage("SiegeUI", {{"UI", "hide"}} )
	elseif msg.name == "SetPlayerPoints" then
		UI:SendMessage("SiegeUI", {{"PlayerPoints_t"..msg.param1.."_p"..msg.param2, msg.paramStr  }} )
	elseif msg.name == "SetPlayerName" then
		UI:SendMessage("SiegeUI", {{"PlayerName_t"..msg.param1.."_p"..msg.param2, msg.paramStr  }} )
		UI:SendMessage("SiegeDrop", {{"user", msg.paramObj  }} )
		UI:SendMessage("SiegeJoin", {{"user", msg.paramObj  }} )
	elseif msg.name == "StoreClientPlayer" then
		UI:SendMessage("SiegeBigTxt", {{"bigtxtVisible", "hide" }} )
		UI:SendMessage("SiegeJoin", {{"clearGUI", true }} )
		UI:SendMessage("SiegeUI", {{"clearNames", true}} )
		
		UI:SendMessage("SiegeText", {{"UI", "hide" }} )
		UI:SendMessage("SiegeJoin", {{"UI", "show" }} )
		UI:SendMessage("SiegeUI", {{"UI", "hide"}} )
		UI:SendMessage("SiegeUI_Attack", {{"siege_showhide", "hide"  }})
		UI:SendMessage("SiegeUI_Defend", {{"siege_showhide", "hide"  }})		
		
		UI:SendMessage("SiegeDrop", {{"user", msg.paramObj  }} )
		UI:SendMessage("SiegeJoin", {{"user", msg.paramObj  }} )
	elseif msg.name == "FRespawn" then
		 msg.paramObj:Resurrect()
	elseif msg.name == "ShowDropButton" then
		 UI:SendMessage("SiegeDrop", {{"sgVisible", "show"  }} )
	elseif msg.name == "HideDropButton" then
		 UI:SendMessage("SiegeDrop", {{"sgVisible", "hide"  }} )
	elseif msg.name == "SetAnimationSet" then
		msg.paramObj:SetAnimationSet{strSet = "carry", bPush=true } 
		local eChangeType = "PUSH"
		msg.paramObj:SetStunned{ StateChangeType = eChangeType,
						bCantMove = false, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	elseif msg.name == "ReSpawnTimer" then
			respawnTime = msg.param1
			self:SetVar("Con.rSpawn",respawnTime )  
			UI:SendMessage("SiegeUI", {{"sgRespawn", "show"}}  )
			UI:SendMessage("SiegeUI", {{"sgRespawnTime", tostring(respawnTime) }}  )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "SpawnTimer" , self )	
	elseif msg.name == "reSetAnimationSet" then
		  msg.paramObj:SetAnimationSet{strSet = ""} 
		  local eChangeType = "POP"
		  msg.paramObj:SetStunned{ StateChangeType = eChangeType,
						bCantMove = false, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	elseif msg.name == "SetPlayerColors" then
			for i = 1, msg.param1 do

					for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
						local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
						local TeamColor = self:MiniGameGetTeamColor{teamID = i}.color
						if GAMEOBJ:GetZoneControlID():MiniGameGetTeam{ playerID = player}.teamID == 1 then
							 colors = 1
						else
							 colors = 0
						end
						player:SwapColor{ bodyPiece = 1 , color = colors }  -- Set Chest Color 
						player:SwapColor{ bodyPiece = 2 , color = colors }  -- Set Legs Color 

					end
		   end
	elseif msg.name == "SendStartTimer" then
		if msg.param1 == 0 then
			UI:SendMessage("SiegeUI", {{"sgtime",  " " }} )

		else
			UI:SendMessage("SiegeUI", {{"sgtime",  tostring(msg.param1) }} )
		end
	elseif msg.name == "FreezPlayer" then
			local eChangeType = "PUSH"
			msg.paramObj:SetStunned{ StateChangeType = eChangeType,
						bCantMove = true, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	elseif msg.name == "unFreezPlayer" then
			local eChangeType = "POP"
			msg.paramObj:SetStunned{ StateChangeType = eChangeType,
						bCantMove = true, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	elseif msg.name == "unFreezAllPlayers" then
			for i = 1, msg.param1 do

				  for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
					  local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
					  local eChangeType = "POP"
								player:SetStunned{ StateChangeType = eChangeType,
								bCantMove = true, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
				  end
			end
	elseif msg.name == "FreezAllPlayers" then
			for i = 1, msg.param1 do
				  for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
					  local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
						local eChangeType = "PUSH"
								player:SetStunned{ StateChangeType = eChangeType,
								bCantMove = true, bCantTurn = false, bCantAttack = true, bCantEquip = true, bCantInteract = true }
				  end
			end
	elseif ( msg.name == "equip" ) then
		local config = {  { "equip", true },  { "owner", "|" .. msg.paramObj:GetID() } ,{"custom_script_server", "scripts/ai/MINIGAME/SIEGE/OBJECTS/LOOT_CAPTURE_OBJECT_SERVER.lua" } }   
		RESMGR:LoadObject { objectTemplate = msg.param1, 

											owner = self,
											rw = 1.0,
											bIsLocalPlayer = false,
											bDroppedLoot = false,
											configData = config }
	elseif ( msg.name == "unquip" ) then
				player:UnEquipItem{ bImmediate = true }
				player:ResetPrimaryAnimation{}
				player:ResetSecondaryAnimation{}
	elseif msg.name == "SendCaptured" then
			UI:SendMessage("SiegeUI", {{"sgcap"..msg.param1, true }})
	elseif msg.name == "resetCaptured" then
			UI:SendMessage("SiegeUI", {{"sgcapreset", true }})
	elseif msg.name == "ShowPlayerStats" then
			UI:SendMessage("SiegeUI", {{"sgopenchart", "Show" }})
	elseif msg.name == "HidePlayerStats" then
			UI:SendMessage("SiegeUI", {{"sgopenchart", "Hide" }})
	elseif  msg.name == "Show_HelpScreen" then
			UI:SendMessage("SiegeUI_Attack", {{"siege_showhide", "show"  }})
			UI:SendMessage("SiegeUI_Defend", {{"siege_showhide", "show"  }})
	elseif msg.name == "Hide_HelpScreen" then
			UI:SendMessage("SiegeUI_Attack", {{"siege_showhide", "hide"  }})
			UI:SendMessage("SiegeUI_Defend", {{"siege_showhide", "hide"  }})
	elseif msg.name == "SendTxt_TeamMsgbox" then
				UI:SendMessage("SiegeUI", {{"sgmsg", msg.paramStr  }})
				print(msg.paramStr )
	elseif msg.name == "Send_State" then
					UI:SendMessage("SiegeUI", {{"sgstate", msg.paramStr  }})
	elseif msg.name == "UISetBkGrndColor" then
		  if (msg.param1 == 1) then
			 UI:SendMessage("SiegeUI", {{"sgbckgrnd", true },{"sgteamID", "1"}} )
		  elseif (msg.param1 == 2) then
			 UI:SendMessage("SiegeUI", {{"sgbckgrnd", true },{"sgteamID", "2"}} )
		  end
	elseif msg.name == "SwapTeamtxt" then
					UI:SendMessage("SiegeUI", {{"swapteamtxt", true }} )
	elseif msg.name == "HideQueUI" then
				UI:SendMessage("SiegeJoin", {{"UI", "hide" }} )
			UI:SendMessage("SiegeUI", {{"UI", "show"}} )
	elseif msg.name == "ShowHUDUI" then
			UI:SendMessage("SiegeUI", {{"UI", "show"}} )
	elseif msg.name == "ShowScoreBoardUI" then
			UI:SendMessage("SiegeBigTxt", {{"bigtxtVisible", "hide" }} )
			UI:SendMessage("SiegeUI", {{"UI", "show"}} )
			UI:SendMessage("SiegeUI", {{"sgopenchart", "Show" }})
			UI:SendMessage("SiegeUI", {{"hideHud", true}})
			
	elseif msg.name == "TimeToBeat" then
				local time = tonumber(msg.paramStr)
				UI:SendMessage("SiegeUI", {{"sgtimetobeat",SecondsToClock(time) }} )
	elseif msg.name == "UISetObjective" then
				if (msg.param1 == 1) then
					UI:SendMessage("SiegeUI", {{"sgbckgrnd", true },{"sgteamID", "1"}} )
				elseif (msg.param1 == 2) then
					UI:SendMessage("SiegeUI", {{"sgbckgrnd", true },{"sgteamID", "2"}} )
				end
				UI:SendMessage("SiegeUI", {{"siegeObjective", msg.paramStr }} )
				UI:SendMessage("SiegeUI_Attack", {{"siegeObjective", msg.paramStr }} )
				UI:SendMessage("SiegeUI_Defend", {{"siegeObjective", msg.paramStr }} )
				
	elseif msg.name == "SetUserObj" then
				UI:SendMessage("SiegeUI", {{"setUserObj", msg.paramStr }} )
	elseif msg.name == "sendTo_Team_1" then 
			for x = 1, #self:MiniGameGetTeamPlayers{teamID = 1}.objects do  
				local player = self:MiniGameGetTeamPlayers{teamID = 1}.objects[x]
				local text = msg.paramStr
			   -- print(text)
				player:DisplayTooltip { bShow = true, strText = text, iTime = 2000 }
			end
	elseif msg.name == "sendTo_Team_2" then 
			for x = 1, #self:MiniGameGetTeamPlayers{teamID = 2}.objects do  
				local player = self:MiniGameGetTeamPlayers{teamID = 2}.objects[x]
				local text = msg.paramStr
			   -- print(text)
				player:DisplayTooltip { bShow = true, strText = text, iTime = 2000 }
			end
	elseif msg.name == "respawn" then
		 GAMEOBJ:GetTimer():AddTimerWithCancel( msg.param1 , "smashed", self )
	elseif msg.name == "SetRespawnTime" then
		self:SetVar("RespawnTime", msg.param1 ) 
	elseif ( msg.name == "Red_Spawn" ) then
			storeObjectByName(self, "Red_Spawn_"..self:GetVar("Con.Red_Spawners"), msg.paramObj)
			local i = self:GetVar("Con.Red_Spawners") + 1
			self:SetVar("Con.Red_Spawners", i ) 
	elseif ( msg.name == "Blue_Spawn" ) then
			storeObjectByName(self, "Blue_Spawn_"..self:GetVar("Con.Blue_Spawners"), msg.paramObj)
			local i =  self:GetVar("Con.Blue_Spawners") + 1
			self:SetVar("Con.Blue_Spawners", i ) 
	elseif ( msg.name == "EndRoundTxt" ) then
	
		UI:SendMessage("SiegeText", {{"UI", "hide" }} )
	   UI:SendMessage("SiegeUI", {{"UI", "hide" }} )
	    UI:SendMessage("SiegeBigTxt", {{"bigtxtVisible", "show" }} )
		UI:SendMessage("SiegeBigTxt", {{"bigtxt", msg.paramStr }} )
	elseif ( msg.name == "SiegeRewards" ) then
	  	UI:SendMessage("SiegeUI", {{"UI", "hide" }} )
		UI:SendMessage("SiegeRewards", {{"rewardsVisible", "show" }} )
	
	elseif (msg.name == "SiegeRewardsHide") then
		UI:SendMessage("SiegeRewards", {{"rewardsVisible", "hide" }} )
	
		
		
		--UI:SendMessage("ToggleLeaderboard", {{"displayData",true}} )
		
		
	end
 
end



function onChildLoaded(self,msg)

	if msg.templateID == 6600  then
	    storeObjectByName(self, "MyLoodOBJ", msg.childID)
	
	end

end

onTimerDone = function(self, msg)

	if msg.name == "SpawnTimer" then
	
		self:SetVar("Con.rSpawn", self:GetVar("Con.rSpawn") - 1)  
	
		local rtime = self:GetVar("Con.rSpawn") 
		
		if rtime <= 0 then
			UI:SendMessage("SiegeUI", {{"sgRespawn", "hide"}}  )
			self:SetVar("Con.rSpawn", 0 )
		else
			UI:SendMessage("SiegeUI", {{"sgRespawnTime", tostring(rtime) }}  )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "SpawnTimer" , self )	
		end


	end


	
end
function onPlayerReady(self,msg)
    
        for i = 1, 2 do
           
                for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
                    local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
                    local TeamColor = self:MiniGameGetTeamColor{teamID = i}.color
          			if GAMEOBJ:GetZoneControlID():MiniGameGetTeam{ playerID = player}.teamID == 1 then
          				 colors = 1
                    else
                    	 colors = 0
                    
                    end
                    
                    player:SwapColor{ bodyPiece = 1 , color = colors }  -- Set Chest Color 
                    player:SwapColor{ bodyPiece = 2 , color = colors }  -- Set Legs Color 
 
                end
       end	
end


----------------------------------------------------------------
-- StopActivity message

-- scoreVar = ( total Score )
-- value1Var = (capts)
-- value2Var = (won)
-- value3Var = (lost)



----------------------------------------------------------------
function onSendActivitySummaryLeaderboardData(self, msg)

  
	 
	
end
