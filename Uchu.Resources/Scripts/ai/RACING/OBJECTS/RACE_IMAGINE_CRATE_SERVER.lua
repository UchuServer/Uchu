local localskillID = 586

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onDie(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end
	
	--print("onDie Imagination")
	
	local target = msg.killerID

    self:PlayFXEffect{effectType = "pickup"}
    
    -- tell the killer's car to add imagination
    target:CastSkill{skillID = localskillID, optionalTargetID = target}
	target:RacingPlayerEvent{ eventType="POWERUP_IMAGINATION", playerID=target, objectID=self }
	target:RacingPlayerEvent{ eventType="SMASHED_SOMETHING", playerID=target, objectID=self }

	local driver = target:GetPossessor().possessorID
	driver:UpdatePlayerStatistic{ updateID = 22 }

	self:SetVar("bIsDead", true)

end
