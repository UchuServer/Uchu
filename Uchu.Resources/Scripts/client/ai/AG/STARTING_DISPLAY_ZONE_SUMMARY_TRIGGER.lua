-- Something touches the phantom object
function onCollisionPhantom(self, msg)                                                      
    -- exclusion checks        
    if msg.objectID:GetID() ~= GAMEOBJ:GetLocalCharID() or msg.objectID:GetFlag{iFlagID = 32}.bFlag then return end
    
    local player = msg.objectID
    -- set flag to true so we know the player has alread done this
    player:SetFlag{iFlagID = 32, bFlag = true}                  
    -- display zone summary
    player:DisplayZoneSummary{sender = self, isZoneStart = true}
end


