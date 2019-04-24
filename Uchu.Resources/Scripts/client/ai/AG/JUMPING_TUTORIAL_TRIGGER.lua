require('o_mis')

local oneTimeOnly = false;

-- Something touches the phantom object
function onCollisionPhantom(self, msg)                                                      
   
    
    -- define player    
    local playerID = GAMEOBJ:GetLocalCharID()
    
    if (msg.objectID:GetID() == playerID and oneTimeOnly == false) then
       
        local player = msg.objectID
                   
	-- 26 is the jumping controls tutorial
        player:Help{ iHelpID = 26 }
	oneTimeOnly = true
        
    end
end


