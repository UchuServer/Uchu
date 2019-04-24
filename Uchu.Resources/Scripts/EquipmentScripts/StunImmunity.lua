
function onStartup(self) 
	self:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunAttack = true, bImmuneToInterrupt = true} -- Make immune to stuns
    self:SetStatusImmunity{ StateChangeType = "PUSH", bImmuneToPullToPoint = true, bImmuneToKnockback = true } -- Make immune to move/teleport behaviors
end
