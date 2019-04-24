require('o_mis')


-- Generic Male is 3385
-- 110 Tame a Pet Mission
-- 111 Collect Gold Bricks Mission

--[[
Null: Player has never had and does not currently have the mission (aka "Available"). MissionState = 1 
Accepted: Player has taken the mission from the mission giver, but has not completed all of the tasks (aka "Active"). MissionState = 2 
Ready to Complete: Player has accepted the mission, and all of the tasks are complete, but player has not accepted the reward. MissionState = 4 
Completed: Player has completed the mission and accepted the reward. MissionState = 8 
Completed and Available: Player has completed the mission at least once, and it is a repeatable mission, ready to be started again. MissionState = 9 
Completed and Active: Player has completed the mission at least once, and has begun it again, but not completed all tasks. MissionState = 10 
Completed and Ready to Complete: Player has completed the mission at least once, and has completed it again, but has not accepted the reward. MissionState = 12 
Failed: Player has failed the mission and is experiencing great sadness. MissionState = 16 
Completed and Reported: The mission has been completed one or more time, and Wolf Blitzer has talked about it. (?Actual definition coming soon?) MissionState = 32 
]]--

-- GLOBALS --
local FAR_RADIUS = 40
local NEAR_RADIUS = 20

--------------------------------------------------------------
function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
    self:SetProximityRadius{ radius = FAR_RADIUS, name = "FAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"
    self:SetProximityRadius{ radius = NEAR_RADIUS, name = "NEAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"

end

--  function IsLocalCharacter(target) -- Get the ID of the target that triggered this message
--      return GAMEOBJ:GetLocalCharID() == target:GetID() -- CLIENT ONLY Get the player object, check to see if it's the same as the ID of the target

--  end


function onProximityUpdate(self, msg) -- Generic male is approached
        
        local targetID = msg.objId --GGJ Define targetID as msg.objID (the thing that sent the message, which is hopefully the player)
        local TameMissionStatus = targetID:GetMissionState{missionID = 110}.missionState --define myMissionState as the current state of mission 136, for the TargetID
        local PetFoodMissionStatus = targetID:GetMissionState{missionID = 111}.missionState --define myMissionState as the current state of mission 136, for the TargetID
        --print ("TameMission Status is ".. TameMissionStatus)
        --print ("PetFoodMission Status is ".. BrickMissionStatus)
        
    if (msg.status == "ENTER" and msg.name == "FAR_MESSAGE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction) then --if the Proximity Message was "Enter" in the far radius and the message was sent by the player, then
                
                --Taming Mission Messages
                if (TameMissionStatus == 1) then --GGJ  Mission has never been started
                    self:DisplayChatBubble{wsText = Localize("AREN'T_THEY_CUTE?")} 
              
                elseif (TameMissionStatus == 2) then --GGJ Mission is "accepted"
                    self:DisplayChatBubble{wsText = Localize("PICK_THE_PET_YOU_WANT,_AND_GIVE_IT_A_CLICK!")}  
           
                elseif (TameMissionStatus == 4) then -- Mission is done, but player hasn't returned to Rancher yet
                    self:DisplayChatBubble{wsText = Localize("YOU_TAMED_ONE!")}  
                    
                --Pet Food Mission Messages
                elseif (PetFoodMissionStatus == 2) then --Mission is "accepted"
                    self:DisplayChatBubble{wsText = Localize("A_PET_CAN_HELP_IN_LOTS_OF_WAYS.")}  
           
                elseif (PetFoodMissionStatus == 4) then --Mission is done, but player hasn't returned to Rancher yet
                    self:DisplayChatBubble{wsText = Localize("SEE_THE_PET_PAW_ICON?")}  
               
               
               end
    
    elseif (msg.status == "ENTER" and msg.name == "NEAR_MESSAGE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction) then --if the Proximity Message was "Enter" in the near radius and the message was sent by the player, then

                --Taming Mission Messages
                if (TameMissionStatus == 1) then --GGJ  Mission has never been started
                    self:DisplayChatBubble{wsText = Localize("CHECK_IN_WITH_THE_PET_RANCHER_FOR_INSTRUCTIONS.")} 
                
                elseif (TameMissionStatus == 2) then --GGJ Mission is "accepted," etc.
                    self:DisplayChatBubble{wsText = Localize("IN_THE_PET-TAMING-MINI-GAME,_YOU_MUST_CHOOSE_THE_BRICKS_THAT_BELONG_IN_THE_MODEL.")}
           
                elseif (TameMissionStatus == 4) then --Mission is done, but player hasn't returned to Rancher yet
                    self:DisplayChatBubble{wsText = Localize("RETURN_TO_THE_PET_RANCHER_FOR_YOUR_NEXT_TASK.")}  
                
                --PetFood Mission Messages
                elseif (PetFoodMissionStatus == 2) then --GGJ Mission is "Completed," etc.
                    self:DisplayChatBubble{wsText = Localize("PETS_CAN_SNIFF_OUT_HIDDEN_TREASURE,_AND_DIG_IT_UP_FOR_YOU.")}
           
                elseif (PetFoodMissionStatus == 4 or PetFoodMissionStatus == 8) then --Mission is done, but player hasn't returned to Rancher yet
                    self:DisplayChatBubble{wsText = Localize("USE_THE_PET_PAW_ICON_TO_GIVE_YOUR_PET_COMMANDS!")}                  
                                    
                end
    
    
    end

end
