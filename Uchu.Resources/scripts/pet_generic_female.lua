require('o_mis')

--------------------------------------------------------------
function onStartup(self) 

    self:SetProximityRadius { radius = 20 , name = "conductRadius" }

end

function IsLocalCharacter(target)

    return GAMEOBJ:GetLocalCharID() == target:GetID()

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

function onProximityUpdate(self, msg)

      if msg.objType == "Enemies" or msg.objType == "NPC" then
        if  msg.name == "conductRadius" and msg.status == "ENTER" and IsLocalCharacter(msg.objId) and msg.objId:GetFaction().faction == 1 then
               
               local myMissionState = msg.objId:GetMissionState{missionID = 109}.missionState
               local myMissionState2 = msg.objId:GetMissionState{missionID = 110}.missionState
               local myMissionState3 = msg.objId:GetMissionState{missionID = 111}.missionState
               local myMissionState4 = msg.objId:GetMissionState{missionID = 112}.missionState
               local myMissionState5 = msg.objId:GetMissionState{missionID = 113}.missionState
               
              
                if myMissionState == 1 or myMissionState == 9 then -- STATE_AV AILABLE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M1_AVAILABLE") } -- "Hi, Click me if you need any help!" 
                 
                end
                if myMissionState == 2  or myMissionState == 10 then-- STATE_ACTIVE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M1_ACTIVE") } -- "Need help finding a Pet?"
               
                end               
                if myMissionState == 4 or myMissionState == 12 then -- READY_TO_COMPLETE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M1_READYTOCOMPLETE") } -- "You got your Egg!"
                
                end              
                ---------------- Mission 2

                if myMissionState2 == 2  or myMissionState2 == 10 then-- STATE_ACTIVE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M2_ACTIVE") } -- "Ah, you've got your egg!"
               
                end               
                if myMissionState2 == 4 or myMissionState2 == 12 then -- READY_TO_COMPLETE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M2_READYTOCOMPLETE") } -- "You got your Pet!"
                
                end        
                ---------------- Mission 3

                if myMissionState3 == 2  or myMissionState3 == 10 then-- STATE_ACTIVE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M3_ACTIVE") } -- "Pets won't behave if ya don't feed'em."
               
                end               
                if myMissionState3 == 4 or myMissionState3 == 12 then -- READY_TO_COMPLETE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M3_READYTOCOMPLETE") } -- "Great Job!, Pet Trainer."
                
                end        
                ---------------- Mission 4

                if myMissionState4 == 2  or myMissionState4 == 10 then-- STATE_ACTIVE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M4_ACTIVE") } -- "Give your Pet some rest."
               
                end               
                if myMissionState4 == 4 or myMissionState4 == 12 then -- READY_TO_COMPLETE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M4_READYTOCOMPLETE") } -- "Great Job!"
                
                end  
                ---------------- Mission 5

                if myMissionState5 == 2  or myMissionState5 == 10 then-- STATE_ACTIVE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M5_ACTIVE") } -- "Where's is your Pet"
               
                end               
                if myMissionState5 == 4 or myMissionState5 == 12 then -- READY_TO_COMPLETE
                self:DisplayChatBubble{wsText = Localize("NPC_PET_GEN_M5_READYTOCOMPLETE") } -- "Ah, Cute Pet!"
                
                end 
            
            end
        end

end




function onClientUse(self, msg)

   local targetID = msg.user 				-- Target OBJ ID 
 
   
   
               local myMissionState = targetID:GetMissionState{missionID = 109}.missionState
               local myMissionState2 = targetID:GetMissionState{missionID = 110}.missionState
               local myMissionState3 = targetID:GetMissionState{missionID = 111}.missionState
               local myMissionState4 = targetID:GetMissionState{missionID = 112}.missionState
               local myMissionState5 = targetID:GetMissionState{missionID = 113}.missionState
               
              
                if myMissionState == 1 or myMissionState == 9 then -- STATE_AV AILABLE
                   targetID:DisplayTooltip{ bShow = true, strText = "Talk to the Pet Rancher", iTime = 0 }
                 
                end
                if myMissionState == 2  or myMissionState == 10 then-- STATE_ACTIVE
                  targetID:DisplayTooltip{ bShow = true, strText = "There are 3 types of pets to choose from. Take your pick; then give it a click!", iTime = 0 }
               
                end               
                if myMissionState == 4 or myMissionState == 12 then -- READY_TO_COMPLETE
                  targetID:DisplayTooltip{ bShow = true, strText = "Take your egg to the Pet Rancher!", iTime = 0 }
                
                end              
                ---------------- Mission 2

                if myMissionState2 == 2  or myMissionState2 == 10 then-- STATE_ACTIVE
                  targetID:DisplayTooltip{ bShow = true, strText = "Now find a nest. Head for those rocky cliffs.", iTime = 0 }
               
                end               
                if myMissionState2 == 4 or myMissionState2 == 12 then -- READY_TO_COMPLETE
                  targetID:DisplayTooltip{ bShow = true, strText = "Now take your new friend back to the Pet Rancher.", iTime = 0 }
                
                end  
                ---------------- Mission 3

                if myMissionState3 == 2  or myMissionState3 == 10 then-- STATE_ACTIVE
                  targetID:DisplayTooltip{ bShow = true, strText = "Drage pet food from your backpack onto the pet. Then Use the STAR Icon to choose a pet command. Tell if to perform an EMOTE action.", iTime = 0 }
               
                end               
                if myMissionState3 == 4 or myMissionState3 == 12 then -- READY_TO_COMPLETE
                  targetID:DisplayTooltip{ bShow = true, strText = "Return to the Pet Rancher", iTime = 0 }
                
                end 
                ---------------- Mission 4

                if myMissionState4 == 2  or myMissionState4 == 10 then-- STATE_ACTIVE
                  targetID:DisplayTooltip{ bShow = true, strText = "Yer pet will follow ya everywhere. If you need a break, choose the GO HOME pet command. ", iTime = 0 }
               
                end               
                if myMissionState4 == 4 or myMissionState4 == 12 then -- READY_TO_COMPLETE
                  targetID:DisplayTooltip{ bShow = true, strText = "Return to the Pet Rancher", iTime = 0 }
                
                end               
                ---------------- Mission 5

                if myMissionState5 == 2  or myMissionState5 == 10 then-- STATE_ACTIVE
                  targetID:DisplayTooltip{ bShow = true, strText = "Just click the STAR Icon to bring your pet back. ", iTime = 0 }
               
                end               
                if myMissionState5 == 4 or myMissionState5 == 12 then -- READY_TO_COMPLETE
                  targetID:DisplayTooltip{ bShow = true, strText = "Take your pet to the Pet Rancher.", iTime = 0 }
                
                end  
 
end 

