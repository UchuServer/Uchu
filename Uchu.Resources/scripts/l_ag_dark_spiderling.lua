require('L_SUSPEND_LUA_AI')
function onStartup(self) 
    self:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunAttack = true, bImmuneToInterrupt = true, bImmuneToStunMove = true, bImmuneToStunTurn = true, bImmuneToStunUseItem = true, bImmuneToStunEquip = true, bImmuneToStunInteract = true} -- Make immune to stuns
    self:SetStatusImmunity{ StateChangeType = "PUSH", bImmuneToPullToPoint = true, bImmuneToKnockback = true } -- Make immune to knockbacks and pulls
    
    -- turn off lua ai
    suspendLuaAI(self)

end

function onGetActivityPoints(self, msg)
	msg.points = 200
	return msg
end

function onDie(self, msg)
	GAMEOBJ:GetZoneControlID():NotifyObject{ name="Spiderling", param1 = 200, ObjIDSender = msg.killerID}
end

