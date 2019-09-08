require('o_mis')

local oneTimeOnly = false;

-- Something touches the phantom object
function onCollisionPhantom(self, msg)                                                      
   
    
    -- define player    
    local playerID = GAMEOBJ:GetLocalCharID()
    
    if (msg.objectID:GetID() == playerID and oneTimeOnly == false) then
       
        local player = msg.objectID
                   
	-- 37 is the Modular Build Start tutorial
        player:Help{ iHelpID = 37 }
        
	oneTimeOnly = true
        
    end
end



