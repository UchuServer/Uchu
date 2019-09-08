require('o_mis')

local oneTimeOnly = false;

-- Something touches the phantom object
function onCollisionPhantom(self, msg)                                                      
    targetID = msg.objectID
    local BobMissionStatus = targetID:GetMissionState{missionID = 173}.missionState  --Check the Bob mission to see the status
    
    -- define player    
    local playerID = GAMEOBJ:GetLocalCharID()
    if (BobMissionStatus > 1) then --If the player has not accepted the mission, print the hint text.
        if (msg.objectID:GetID() == playerID and oneTimeOnly == false) then
           
            local player = msg.objectID
                       
        -- 28 is the camera controls tutorial
            player:Help{ iHelpID = 28 }
        oneTimeOnly = true
            
        end
    end
end



