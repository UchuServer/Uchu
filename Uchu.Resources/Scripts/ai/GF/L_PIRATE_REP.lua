

function onMissionDialogueOK(self, msg)  
     if msg.bIsComplete == true and msg.missionID == self:GetVar("missionID") then    --msg.missionID == self:GetVar("missionID") --set in config data on object in HF
          print ("turned in mission")            
          local player = msg.responder      
            
          if (player:GetFlag{iFlagID = 24}.bFlag == true) then return end  
            
          --set flag to true so we know the player has alread done this  
          player:SetFlag{iFlagID = 24, bFlag = true}   
     end   
end