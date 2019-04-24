local bHasBeenUsed = false


-- GLOBALS --
local NEAR_RADIUS = 15

--------------------------------------------------------------
function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
    self:SetProximityRadius{ radius = NEAR_RADIUS, name = "NEAR_MESSAGE" } -- Define the message for the Far Radius as "Far_Message"
    --print ("script started.")
end



function onProximityUpdate(self, msg) 
    --print ("proximity update")

    local player = msg.objId-- get the ID of the thing that sent the proximity message
    
        
    if (msg.status == "ENTER" and msg.name == "NEAR_MESSAGE" and msg.objId:GetFaction().faction == 1) then --if the Proximity Message was "Enter" in the near radius and the message was sent by the player, then
    --print ("got past if")
         
    player:DisplayTooltip{ bShow = true, strText = "This is a Pet Blocker. To see Pet Blockers in action, come back next sprint.", iTime = 0 } --display hint message and leave it on until player turns it off
    
    bHasBeenUsed = true; --Don't ever display this tooltip again
        
    end
        
end        
