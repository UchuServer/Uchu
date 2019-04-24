require ('/ai/PETS/L_PET_BOUNCER_CLIENT')



function onCollisionPhantom(self, msg) -- Someting touches the Pet Switch
        --print ("Message on collision")
        
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) -- Get the player
        --local player = msg.objectID -- get the ID of the thing that sent the collision message
        local TameMissionStatus = player:GetMissionState{missionID = 110}.missionState --define myMissionState as the current state of mission 136, for the TargetID
        --print ("TameMission Status is ".. TameMissionStatus)
        
       
               --Taming Mission Messages
               if ((TameMissionStatus > 2) and (TameMissionStatus < 8)) then --GGJ  Mission has not yet been completed
                  
                player:DisplayChatBubble{wsText = Localize("HOLD THE PHONE! I need to check in with the Pet Rancher.")} 
              
               
               end
      
end



