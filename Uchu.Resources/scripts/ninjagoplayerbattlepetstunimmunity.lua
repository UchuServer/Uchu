----------------------------------------
-- Generic Server side script that casts 
-- the NJ Buff on self so enemy NJ skills can react to this buff.
-- Also makes the summoned pet immune to movement skill behaviors like stuns and knockbacks.
-- created ME... 6/9/11

require('L_SUSPEND_LUA_AI')
function onStartup(self) 
    -- Applies the strongest player Ninjago buff to reduce enemy damage
    self:CastSkill{skillID = 1491}
    -- turn off lua ai
    suspendLuaAI(self)	
    -- Make immune to stuns    
    self:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunAttack = true} 
    -- Make immune to move/teleport behaviors
    self:SetStatusImmunity{ StateChangeType = "PUSH", bImmuneToPullToPoint = true, bImmuneToKnockback = true, bImmuneToInterrupt = true } 
end
