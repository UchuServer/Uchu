require('o_mis')

local oneTimeOnly = false;


-- Something touches the phantom object
function onCollisionPhantom(self, msg) 
    --print("Collision Detected")
   
    
    -- define player    
    local playerID = GAMEOBJ:GetLocalCharID()
    
    -- un-set this tutorial flag so that we can play it again in another level.
    --playerID:SetFlag{iFlagID = 27, bFlag = false }
    
    -- if the collision came from the player, and oneTimeOnly hasn't been set yet
    if (msg.objectID:GetID() == playerID and oneTimeOnly == false) then
        
        --print("inside if statement")
        
        -- define player again for some reason
        local player = msg.objectID
        
        -- Set the player flag 27 as false so that it thinks it hasn't been played before
        player:SetFlag{iFlagID = 28, bFlag = false }
        
                   
        -- 27 is the double jumping controls tutorial
        player:Help{ iHelpID = 28 }
        oneTimeOnly = true
    
        
    end
end



