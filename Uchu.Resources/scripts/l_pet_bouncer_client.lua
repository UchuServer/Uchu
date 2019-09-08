require('L_BOUNCER_BASIC')
--client-side bouncer script
require('o_mis')
--local bHasBeenUsed = false
--local SWITCH_HINT_RADIUS = 6
--local bCINEMA_ONCE = false
CONSTANTS = {}
--CONSTANTS["NO_OBJECT"] = "0"
CONSTANTS["effectNum"] = 291
CONSTANTS["effectName"] = "bouncer"

------------------------------------------------------------------------------------------
-- Note: Hint messages have been removed from this script. They will be handled in code. -
------------------------------------------------------------------------------------------

function onStartup(self)
    --print ("Cine Script Started!")
    --self:SetProximityRadius{ radius = SWITCH_HINT_RADIUS, name = "HINT_MESSAGE" } --Set radius to detect if player is close (for hint message)

    self:SetVar("storedOnce", false) 

end
    

function onBouncerTriggered(self, msg)

	--Hackish fix for the 'bounce collision'
	local player = msg.triggerObj
	local objPos = player:GetPosition().pos
	objPos.y = objPos.y + 1
	--player:DisplayChatBubble{wsText = (objPos.x .. " " .. objPos.y .. " " .. objPos.z)}
	player:SetPosition{pos = objPos}
	
	bounceObj(self, msg.triggerObj)
	bHasBeenUsed = true;
end



--Display hint message if player approaches button
function onProximityUpdate(self, msg) -- Switch is approached
       if ( self:GetVar("storedOnce") == false) then
	
			StoreOnce(self)
			self:SetVar("storedOnce", true) 
	
	    end 
        --[[
    if bHasBeenUsed == true then -- Do not display message once the player has successfully used the Pet Bouncer
        return        
    end
        
        local targetID = msg.objId
        
        if (msg.status == "ENTER" and msg.name == "HINT_MESSAGE" and targetID:GetFaction().faction == 1 and PetFoodMissionStatus ~= 2) then 
            targetID:DisplayTooltip{ bShow = true, strText = "A pet can activate this bouncer.", iTime = 2000 }
        end
        
        
        --play a cinematic the first time the player approaches the switch with a pet
        local PetFoodMissionStatus = targetID:GetMissionState{missionID = 111}.missionState --define myMissionState as the current statess of mission 136, for the TargetID
        if (bCINEMA_ONCE == false and PetFoodMissionStatus == 2) then --if the Proximity Message was "Enter" and the message was sent by the player, then
        
        --local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
               --targetID:PlayCinematic { pathName = "Bouncer_Cine" }
               
               --print ("inside if statement")
               --targetID:DisplayTooltip{ bShow = true, strText = "CINEMATIC: Pet needs food. Pet Rancher gives food. Now try clicking your pet", iTime = 10000 }
               bCINEMA_ONCE = true
        end
        ]]--
end

--------------------------------------------------------------
-- Enables an effect on the spout unless one is already present
--------------------------------------------------------------
function EnableEffect(self, name)

    -- return out if we already have an effect
    local myEffect = self:GetVar("Effect")
	if ( myEffect ) then
        return
	end
	
    -- make a new effect
		self:PlayFXEffect{ name = CONSTANTS["effectName"], effectID = CONSTANTS["effectNum"], type = name }

    -- save the effect
		self:SetVar("Effect", true);

end

--------------------------------------------------------------
-- Removes an effect on the spout
--------------------------------------------------------------
function RemoveEffect(self)


    -- get current effect
    local myEffect = self:GetVar("Effect")
    
    -- remove the effect
	if( myEffect )
		self:StopFXEffect{ name = CONSTANTS["effectName"] }

	self:SetVar("Effect", false)
end


function onNotifyObject(self, msg)

	if msg.name == "on" then
		EnableEffect(self, "bounce-fx")
	end 
	
	if msg.name == "off" then
		RemoveEffect(self)
	
	end 
	
end 




function StoreOnce(self)

	self:SetVar("storedOnce", true)

	local switch = self:GetObjectsInGroup{ group = self:GetVar("grp_name") }.objects
	for i = 1, table.maxn (switch) do 
            if ( switch[i]:GetLOT().objtemplate == 3463 ) then
               
		storeObjectByName(self, "pet_switch", switch[i])
        storeObjectByName(switch[i], "pet_bouncer", self)

            end 
	end         
      	
	self:SetVar("SaveObjests", "stored") 
end





---
---
---

--------------------------------------------------------------

--[[
function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
    print ("Cine Script Started!")
    self:SetProximityRadius{ radius = CINE_RADIUS, name = "CINE_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"

end


function onProximityUpdate(self, msg) -- Bouncer is approached
        --print ("Proximity update!")

        local targetID = msg.objId --GGJ Define targetID as msg.objID (the thing that sent the message, which is hopefully the player)
        local PetFoodMissionStatus = targetID:GetMissionState{missionID = 111}.missionState --define myMissionState as the current statess of mission 136, for the TargetID
        --print ("PetFoodMission Status is ".. BrickMissionStatus)

         if (msg.status == "ENTER" and msg.name == "CINE_MESSAGE" and msg.objId:GetFaction().faction == 1 and bCINEMA_ONCE == 0) then --if the Proximity Message was "Enter" and the message was sent by the player, then

               --print (PetFoodMissionStatus)
               
               --Taming Mission Messages
               if (PetFoodMissionStatus ~= 2) then -- Mission is any state other than accepted
                  return
               end
               
               
               --local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
               targetID:PlayCinematic { pathName = "Bouncer_Cine" }
               
               --print ("inside if statement")
               targetID:DisplayChatBubble{wsText = "Cinematic joy!"}  
               bCINEMA_ONCE = 1

               

         end

end
]]--
