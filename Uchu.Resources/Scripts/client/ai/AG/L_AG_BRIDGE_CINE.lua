require('o_mis')

function onClientUse(self, msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

    -- exclusion checks        
    if player:GetImagination().imagination < 1 then return end
    
    player:PlayCinematic { pathName = "Mon_QB_Bridge_01" }
end

function onRebuildCancel(self, msg)
    --print('exit')
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())      
    player:EndCinematic()
end