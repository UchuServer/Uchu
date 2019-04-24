require('o_mis')


-- GLOBALS --
local FAR_RADIUS = 65
local NEAR_RADIUS = 20

--------------------------------------------------------------
function onStartup(self) -- The object with this script component is defined as "self" when the script first starts up
    self:SetProximityRadius{ radius = FAR_RADIUS, name = "FAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"
    self:SetProximityRadius{ radius = NEAR_RADIUS, name = "NEAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"

end


function IsLocalCharacter(target) -- Check to see if the "local character" and the "target" which is triggering this are the same
    return GAMEOBJ:GetLocalCharID() == target:GetID() -- CLIENT ONLY Get the player object, check to see if it's the same as the ID of the target

end


function onGetOverridePickType(self, msg) -- Get the Pick Type (cursor clicking options) for the script object, in preparation of changing it.

    msg.ePickType = 14 -- Set the Pick Type to 14
	return msg -- Send Pick Type 14 back to the script object
    
end


function onProximityUpdate(self, msg)

        local targetID = msg.objId --GGJ Define targetID as msg.objID, which comes from the player
        local myMissionState = targetID:GetMissionState{missionID = 136}.missionState --Find out what the mission state is for the Rescue Tourists Mission, and save it as "myMissionState"

    if msg.status == "ENTER" and msg.name == "FAR_MESSAGE" and msg.objId:GetFaction().faction == 1 then --if the Proximity Message was "Enter" in the far radius and the message was sent by the player, then

        --local targetID = msg.objId --GGJ Define targetID as msg.objID, which comes from the player
        
        --local myMissionState = targetID:GetMissionState{missionID = 136}.missionState --Find out what the mission state is for the Rescue Tourists Mission, and save it as "myMissionState"
                
                if myMissionState == 1 or myMissionState == 9 then --GGJ Mission is "Available" (1) or "Completed & Available" (9)
                self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_FAR_AVAILABLE") } -- "HELP ME! I'M LOST!"
                end

                if myMissionState == 2  or myMissionState == 10  and  not self:GetVar("PlayerFound") then --GGJ Mission is "Accepted" (2) or "Completed & Accepted" (10)
                self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_FAR_ACTIVE") } -- "GET UP HERE AND RESCUE ME!"
                end
                
                --if myMissionState == 4 or myMissionState == 12 then -- GGJ Player has just clicked on 3 tourists and is trying to save a 4th; Mission is "Ready to Complete" (4) or "Completed & Ready to Completed" (12)
                --self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_FAR_READYTOCOMPLETE") } -- "BLAH"
                --end
     end


    if msg.status == "ENTER" and msg.name == "NEAR_MESSAGE" and msg.objId:GetFaction().faction == 1 then --if the Proximity Message was "Enter" in the near radius and the message was sent by the player, then

        --local targetID = msg.objId --GGJ Define targetID as msg.objID, which comes from the player
        
        --local myMissionState = targetID:GetMissionState{missionID = 136}.missionState --Find out what the mission state is for the Rescue Tourists Mission, and save it as "myMissionState"
                
                if myMissionState == 1 or myMissionState == 9 then --GGJ Mission is "Available" (1) or "Completed & Available" (9)
                self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_NEAR_AVAILABLE") } -- "Did the Pet Rancher send you? I don't talk to strangers."
                end

                if myMissionState == 2  or myMissionState == 10  and  not self:GetVar("PlayerFound") then --GGJ Mission is "Accepted" (2) or "Completed & Accepted" (10)
                self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_NEAR_ACTIVE") } -- "Click me you fool!"
                end
                
                if myMissionState == 4 or myMissionState == 12 then -- GGJ Player has just clicked on 3 tourists and is trying to save a 4th; Mission is "Ready to Complete" (4) or "Completed & Ready to Completed" (12)
                self:DisplayChatBubble{wsText = Localize("NPC_PET_MIS_NEAR_READYTOCOMPLETE") } -- "You better check in with the Rancher. He can't handle too many rescues at once."
                end
     end


     
end

-- Script by Trent, Comments by Geoff (to help learn LUA)
-- This script is duplicated in PET_MISSIONS_NPCS.lua, presumably to get the same info to happen on the client.