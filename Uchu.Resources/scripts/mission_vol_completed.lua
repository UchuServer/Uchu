require('o_mis')
function onStartup(self) 
self:SetVar("found", 0)
end


function onCollisionPhantom(self, msg)
   
    local player = msg.senderID
   
      
 
    local faction = player:GetFaction()
    local mstate = player:GetMissionState{missionID = 239}.missionState
    
    if faction and faction.faction == 1 and mstate == 2  then

      player:UpdateMissionTask{ value = 239, value2 = 1, taskType = "complete" }
   end
    
end
