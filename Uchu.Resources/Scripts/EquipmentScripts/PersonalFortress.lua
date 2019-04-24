function onStartup(self) 
    -- store the caster for use when skill is cast.
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "FireSkill", self )
	self:GetParentObj{}.objIDParent:SetStatusImmunity{StateChangeType = "PUSH",bImmuneToKnockback = true, bImmuneToInterrupt = true, bImmuneToSpeed = true, bImmuneToBasicAttack = true, bImmuneToDOT = true, bImmuneToImaginationGain=false, bImmuneToImaginationLoss = true, bImmuneToQuickbuildInterrupt = false, bImmuneToPullToPoint = false}
    self:GetParentObj{}.objIDParent:SetStunImmunity{StateChangeType = "PUSH", bImmuneToStunMove = true, bImmuneToStunTurn = true, bImmuneToStunAttack = true, bImmuneToStunEquip = true, bImmuneToStunInteract = true}
    self:GetParentObj{}.objIDParent:SetStunned{StateChangeType = "PUSH", bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true}
    --print("**************startingup**************")
end


function onDie(self, msg)
   self:GetParentObj{}.objIDParent:SetStatusImmunity{StateChangeType = "POP",bImmuneToKnockback = true, bImmuneToInterrupt = true, bImmuneToSpeed = true, bImmuneToBasicAttack = true,bImmuneToDOT = true, bImmuneToImaginationGain = false, bImmuneToImaginationLoss = true, bImmuneToQuickbuildInterrupt = false, bImmuneToPullToPoint = false}
   self:GetParentObj{}.objIDParent:SetStunImmunity{StateChangeType = "POP", bImmuneToStunMove = true, bImmuneToStunTurn = true, bImmuneToStunAttack = true, bImmuneToStunEquip = true, bImmuneToStunInteract = true}
   self:GetParentObj{}.objIDParent:SetStunned{StateChangeType = "POP", bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true}
   --print("**************I can has on die from the fortress?********")
   --print(self:GetParentObj{}.objIDParent:GetName().name)
end


function onTimerDone(self, msg)
	if msg.name == "FireSkill" then
		self:CastSkill{skillID = 650, optionalOriginatorID = self:GetParentObj{}.objIDParent}
	end
end