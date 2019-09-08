require('o_mis')

 

 

-- Something touches the phantom object

function onCollisionPhantom(self, msg)                                                      

   

    -- define player    

    local playerID = GAMEOBJ:GetControlledID()

 

    

    if (msg.objectID:GetID() == playerID:GetID()) then

       

        local player = msg.objectID

                   

            -- 26 is the jumping controls tutorial

            player:PlayFXEffect{ effectID = 136, effectType = "create", priority = 1.07 }

        

    end

end



