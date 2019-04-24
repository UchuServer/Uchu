--------------------------------------------------------------
-- (SERVER SIDE) Script for the Lead the Monkey Minigame Button
--
-- :: "TargetGroup" config data needs to be set to the group of the game board
--------------------------------------------------------------



--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)

end


--------------------------------------------------------------
-- Called when object is used on the server
--------------------------------------------------------------
function onUse(self, msg)

    -- get the target group config data
    local targetGroup = self:GetVar("TargetGroup")
    if (targetGroup) then
    
        -- store who pressed me for a query later
        storeObjectByName(self, "myUser", msg.user)

        -- send a notify to everyone (should only be a single object)
        local objects = self:GetObjectsInGroup{ group = targetGroup }.objects
        for i = 1, table.maxn (objects) do  
            objects[i]:FireEvent{ args = "button_activate", senderID = self }
        end  
        
    end
    
end
