require('o_mis')


-- GLOBALS --
local FAR_RADIUS = 40
local NEAR_RADIUS = 20
local MESSAGE_NUMBER = 1

--------------------------------------------------------------
function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
    self:SetProximityRadius{ radius = FAR_RADIUS, name = "FAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"
    self:SetProximityRadius{ radius = NEAR_RADIUS, name = "NEAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"
    --print ("Started up!")

end


function onGetOverridePickType(self, msg) -- Get the Pick Type (cursor clicking options) for the script object, in preparation of changing it.

    msg.ePickType = 14 -- Set the Pick Type to 14
	return msg -- Send Pick Type 14 back to the script object
    
end


function onProximityUpdate(self, msg) -- NPC is approached
        --print ("Approached!")
        local targetID = msg.objId --GGJ Define targetID as msg.objID (the thing that sent the message, which is hopefully the player)


    if msg.status == "ENTER" and msg.name == "FAR_MESSAGE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction and MESSAGE_NUMBER == 1 then --if the Proximity Message was "Enter" in the far radius and the message was sent by the player, then
                
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_INVITE_01")} 
                                               
        
    elseif msg.status == "ENTER" and msg.name == "NEAR_MESSAGE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction and MESSAGE_NUMBER == 1 then --if the Proximity Message was "Enter" in the near radius and the message was sent by the player, then

            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_INVITE_02")} 
            
    elseif msg.status == "LEAVE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction and MESSAGE_NUMBER >= 2 then --if the Proximity Message was "Enter" in the near radius and the message was sent by the player, then

            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_INVITE_03")} 
                                    
    end
   
    
end


function onCheckUseRequirements(self, msg)

        --print ("Clicked!")
        --local user = msg.objIDUser	
        --local pet = user:GetPetID().objID
        
		msg.preventRequirementsIcon = true
		if msg.isFromUI then return end
		
        if (MESSAGE_NUMBER == 1) then 
      
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_01")} 
            MESSAGE_NUMBER = (MESSAGE_NUMBER + 1)
            
        
        elseif (MESSAGE_NUMBER == 2) then
        
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_02")} 
            MESSAGE_NUMBER = (MESSAGE_NUMBER + 1)
            
        elseif (MESSAGE_NUMBER == 3) then
        
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_03")} 
            MESSAGE_NUMBER = (MESSAGE_NUMBER + 1)
        
        elseif (MESSAGE_NUMBER == 4) then
        
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_04")} 
            MESSAGE_NUMBER = (MESSAGE_NUMBER + 1)
            
        elseif (MESSAGE_NUMBER == 5) then
        
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_05")} 
            MESSAGE_NUMBER = (MESSAGE_NUMBER + 1)
            
        elseif (MESSAGE_NUMBER == 6) then
        
            self:DisplayChatBubble{wsText = Localize("PET_TUTORIAL_MESSAGE_06")} 
            MESSAGE_NUMBER = 1
                	    
        end
		msg.bCanUse = false
		return msg
    
end 


