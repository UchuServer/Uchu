function onFactionTriggerItemEquipped (self)
    self:AddStatTrigger{Name="Low Imagination", Stat="IMAGINATION", Operator="LESS", Value=1}
end

function onStatEventTriggered(self, msg)
	if msg.Parent:Exists{} then 
		-- the value of the stat
		local statValue = msg.StatValue
		-- Fire the toughness skill
            if statValue ~= msg.OldValue and self:GetVar("scriptedCooldown") == 0 then
                msg.Parent:CastSkill{skillID = 394}
                GAMEOBJ:GetTimer():AddTimerWithCancel( 11 , "skillCooldown", self )
                self:SetVar("scriptedCooldown", 1)
            end
	end
end

function onStartup(self)
    self:SetVar("scriptedCooldown", 0)
end

function onTimerDone(self, msg)
    if msg.name == "skillCooldown" then
        self:SetVar("scriptedCooldown", 0)
    end
end
