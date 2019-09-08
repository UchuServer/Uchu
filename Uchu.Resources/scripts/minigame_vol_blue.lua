require('o_mis')
function onCollisionPhantom (self,msg)

	local player = msg.objectID
 
	if ( player:GetFaction().faction == 101 ) then 
	
        local item = player:GetEquippedItemInfo{ slot = "hair" }.lotID 
          -- Check to see if my flag is here
          
           
        if  item == GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlag_LootOBJ") and  GAMEOBJ:GetZoneControlID():GetVar("Con.Blue_Flag_Home")  then
 
            for i =0,  player:GetInventorySize{inventoryType = 1 }.size do
                if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
                
                    if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate == GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlag_LootOBJ") then
                        local item = player:GetInventoryItemInSlot{slot = i }.itemID
    
                        player:GetInventoryItemInSlot{slot = i }.itemID:UnEquipItem{bImmediate = true}
                        player:RemoveItemFromInventory{ iObjTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlag_LootOBJ") }
                        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = player:GetName().name.." Captured The Red Flag!!" }
                        local team = GAMEOBJ:GetZoneControlID():MiniGameGetTeam{playerID = player}.teamID
                        GAMEOBJ:GetZoneControlID():MiniGameAddTeamScore{teamID = team, scoreType = 0, score = 1 }
                        GAMEOBJ:GetZoneControlID():MiniGameAddPlayerScore{playerID = player, scoreType = 2, score = 1 } -- Cap Score
                        GAMEOBJ:GetZoneControlID():MiniGameAddPlayerScore{playerID = player, scoreType = 4, score = GAMEOBJ:GetZoneControlID():GetVar("Set.CTF_CAPTS") } -- Cap Points
                        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToclient_bubble" , paramStr = "+"..GAMEOBJ:GetZoneControlID():GetVar("Set.CTF_CAPTS").." ("..GAMEOBJ:GetZoneControlID():MiniGameGetPlayerScore{playerID= player, scoreType= 4}.score..")" ,paramObj = player } -- send to Client bubble txt                          
                    
                        GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "CaptedFlag", self )
             

                    end
                end
            end
        end		
    end
end

function onStartup(self, msg)
 
   
end

function onTimerDone(self, msg)
    if msg.name == "CaptedFlag" then
         local team_1 =   GAMEOBJ:GetZoneControlID():MiniGameGetTeamScore{teamID = 1, scoreType = 0 }.score
         local team_2 =   GAMEOBJ:GetZoneControlID():MiniGameGetTeamScore{teamID = 2, scoreType = 0 }.score
         
         GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Blue Team Score! ("..team_1..")   Red Team Score! ("..team_2..")" }
         if  team_1 == GAMEOBJ:GetZoneControlID():GetVar("Set.Score_To_Complete") then
         
                  GAMEOBJ:GetZoneControlID():MiniGameSetTeamScore{teamID = 1, scoreType = 0, score = 0 }
                  GAMEOBJ:GetZoneControlID():MiniGameSetTeamScore{teamID = 2, scoreType = 0, score = 0 }
                  GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr =  "Blue Team Won the Match!!" }
                  GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name="FreezAllPlayers" ,param1 = GAMEOBJ:GetZoneControlID():GetVar("Set.Number_Of_Teams")}
                  GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "MatchWon", self )
        else 
                GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "CaptedFlag2", self )
        end
    end
     if msg.name == "CaptedFlag2" then
  

          GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Reseting Red Flag in 5 Seconds!" }
          GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "CaptedFlag3", self )

    end
    if msg.name == "CaptedFlag3" then
           GAMEOBJ:GetZoneControlID():SetVar("Con.Red_Flag_Home", true)
          local redFlag =  getObjectByName( GAMEOBJ:GetZoneControlID(), "Red_Flag_1")
          local redpos =   redFlag:GetPosition().pos 
          RESMGR:LoadObject { objectTemplate =  GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlagOBJ") , x= redpos.x, y= redpos.y , z= redpos.z, owner =  GAMEOBJ:GetZoneControlID() }
    end
    if msg.name == "MatchWon" then
          GAMEOBJ:GetZoneControlID():SetVar("Con.Red_Flag_Home", true)
          local redFlag =  getObjectByName( GAMEOBJ:GetZoneControlID(), "Red_Flag_1")
          local redpos =   redFlag:GetPosition().pos 
          RESMGR:LoadObject { objectTemplate =  GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlagOBJ") , x= redpos.x, y= redpos.y , z= redpos.z, owner =  GAMEOBJ:GetZoneControlID() }   
           GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = "Starting Match in   15 Seconds!" }
           GAMEOBJ:GetTimer():AddTimerWithCancel( 15 , "restart", self )    
            GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "showpoints", self )  
    end
    if msg.name == "restart" then
    
        GAMEOBJ:GetZoneControlID():NotifyObject{name = "reStartCTF" }
    end
    if msg.name == "showpoints" then

          GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_points" }
    end
end

          
