----------------------------------------------------------
--Script by Devon J -- 8/2/10
-- fills up the player's health, armor and imagination when
-- they enter the instance
----------------------------------------------------------

function onPlayerLoaded(self, msg)

    local player = msg.playerID
    
    local maxHealth = player:GetMaxHealth{}.health
    local maxArmor = player:GetMaxArmor{}.armor
    local maxImagination = player:GetMaxImagination{}.imagination
    
    player:SetHealth{ health = maxHealth }
    --print("set health=" .. player:GetHealth{}.health)
    
    player:SetArmor{ armor = maxArmor }
    --print("set armor=" .. player:GetArmor{}.armor)
    
    player:SetImagination{ imagination = maxImagination }
    --print("set imagination=" .. player:GetImagination{}.imagination)
    
end
