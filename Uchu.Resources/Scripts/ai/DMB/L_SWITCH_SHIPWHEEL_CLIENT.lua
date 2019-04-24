require('o_mis')

local effect_interval = 1.0

function onStartup(self)

    self:SetPickType{ePickType = 14}

end

function onClientUse(self)                      

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    player:PlayAnimation{animationID = "ship-wheel"}
    self:PlayAnimation{animationID = "turn"}

end