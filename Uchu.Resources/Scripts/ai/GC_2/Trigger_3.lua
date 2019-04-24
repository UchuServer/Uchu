



require('o_mis')




function onStartup(self)
    self:SetName { name = self:GetVar("Trigger 1")  }
    self:SetProximityRadius { radius = 50 , name = "BRICKS" }
    self:SetProximityRadius { radius = 15 , name = "BRICKS2" }
    storeMeanderPoint(self)
    self:SetVar("spawned1", 0 ) 
  
end

function onProximityUpdate(self, msg)
    local foundFaction = msg.objId:GetFaction().faction
    if foundFaction == 1 and self:GetVar("StoredPlayerID") == nil then
        StoreRudPeer(self,msg.objId)

    end 
    if foundFaction == 1 and self:GetVar("StoredPlayerID") == nil then
        StoreRudPeer(self,msg.objId)
        self:SetVar("StoredPlayerID", 1)
        
    end 
    
    
    
    if foundFaction == 7 and msg.status == "ENTER" and msg.name == "BRICKS"  then
           
            PoS = getMeanderPoint(self)
               
    
                msg.objId:GoTo { speed = 3,
                            target = {
                                 x = -45  ,
                                 z = -165 ,
                                 y = 913  ,
                           },
                   }
           
    end
    if foundFaction == 7 and msg.status == "ENTER" and msg.name == "BRICKS2"  then
                msg.objId:Teleport{  x=0,y=0,z=0}
                self:SetVar("spawned1", self:GetVar("spawned1") + 1 ) 
                getRudPeer(self):UpdateMissionTask{ target =  msg.objId , value = 1, taskType = "kill" }
                
               if  self:GetVar("spawned1") >= 14 then
                 RESMGR:LoadObject { objectTemplate = 2897  , x = -37.18 , y = 912.89 , z = -165.56 ,owner = self } 
               end
               if  self:GetVar("spawned1") >= 24 and self:GetVar("bb") == nil then
                 self:SetVar("bb","bla") 
                 RESMGR:LoadObject { objectTemplate = 2898  , x = -37.18 , y = 912.89 , z = -165.56 ,owner = self } 
                 RESMGR:LoadObject { objectTemplate = 2864  , x = -25.18 , y = 912.89 , z = -152.56 ,owner = self } 
                 GAMEOBJ:GetZoneControlID():SetVar("MissionDone",true)
                  self:CastSkill{skillID = 72 } 
               end
    end
end


