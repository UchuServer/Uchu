require('o_mis')
require('client/ai/NP/L_NP_NPC')

function onClientUse(self)

    SetMouseOverDistance(self, 100)	                       

    print "Freeze Minifig, turn wheel, play turn animation"

    	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayAnimation{animationID = "ship-wheel"}
        self:PlayAnimation{animationID = "turn"}

end