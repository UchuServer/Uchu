--------------------------------------------------------------
-- (CLIENT SIDE) Friendly Felix in Scene 1
--
-- On proximity of the local character, will approach and 
-- present help screen information for playing the game.
-- When the player has finished reading the help, he will
-- leave and despawn.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('client/ai/NP/L_NP_NPC')

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

end

--------------------------------------------------------------
-- Happens on client interaction
--------------------------------------------------------------
function onClientUse(self, msg)

    -- get the player 
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if (player) and (player:Exists()) then

        -- show help
                
    end
    
end
    




