



require('o_mis')
require('State')



function onStartup(self)
    self:SetName { name = self:GetVar("Trigger 1")  }
    self:SetProximityRadius { radius = 20 , name = "BRICKS" }
    storeMeanderPoint(self)
    
    
    self:UseStateMachine{} 
    Idle = State.create()
    Idle.onEnter = function(self)
    
    end
    Store = State.create()
    Store.onEnter = function(self)

       

         

                myPos = getMeanderPoint(self)
                local PoS = getRandomPos(self,myPos,10)

                 getMyTarget(self):GoTo { speed = 3,
                                  target = { 
                                 x = PoS.x ,
                                 z = PoS.z ,
                                 y = PoS.y,
                           },
                   }

    end 
    Store.onArrived = function(self)
              
    end 
  
    sendAway = State.create()
    sendAway.onEnter = function(self)
     local foundObj = self:GetProximityObjects { name = "BRICKS"}.objects
             for i = 1, table.maxn (foundObj) do  
                   if foundObj[i]:GetFaction().faction == 7 then
                                            
                                foundObj[i]:FollowTarget { targetID = getRudPeer(self) , radius = 3 ,speed = 4, keepFollowing = false }
                              
                    end
              end

    end
    
    sendAway.onArrived = function(self)
              
    end   

  
    addState(Store, "Store", "Store", self)
    addState(sendAway, "sendAway", "sendAway", self)
    addState(Idle, "Idle", "Idle", self)
    beginStateMachine("Idle", self) 
    Idle.onEnter(self)
    
    
end

function onProximityUpdate(self, msg)
    local foundFaction = msg.objId:GetFaction().faction
    if foundFaction == 1 and self:GetVar("StoredPlayerID") == nil then
        StoreRudPeer(self,msg.objId)
        
        local idString = self:GetID()
        local finalID = "|" .. idString
        GAMEOBJ:GetZoneControlID():SetVar("Trigger_A_ID", finalID) 
        self:SetVar("StoredPlayerID", 1)
        
    end 
    
    if foundFaction == 7 and msg.status == "ENTER" and GAMEOBJ:GetZoneControlID():GetVar("Mission_A_State") ~= "travleA" then
            storeTarget(self,  msg.objId)
            msg.objId:SetVar("Imfollowing","done")
            msg.objId:SetVar("PointInLine",nil)
            msg.objId:SetVar("EndOfLineTarget",nil)
            getRudPeer(self):UpdateMissionTask{ target =  msg.objId , value = 1, taskType = "kill" }
            GAMEOBJ:GetZoneControlID():SetVar("Mission_A_Brick",  GAMEOBJ:GetZoneControlID():GetVar("Mission_A_Brick") + 1 )
            
            if GAMEOBJ:GetZoneControlID():GetVar("Mission_A_Brick") == 15 then
             local foundObj = self:GetProximityObjects { name = "BRICKS"}.objects
             for i = 1, table.maxn (foundObj) do  
                   if foundObj[i]:GetFaction().faction == 7 then
                                            
                               foundObj[i]:SetVar("Imfollowing", false) 
                              
                    end
              end
            
            end
            
            setState("Stage1", GAMEOBJ:GetZoneControlID())
            setState("Store", self)

    
    end

end


