local localskillID = 585

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    --local target = msg.objectID
    
	--target:RacingPlayerEvent{ eventType="POWERUP_IMAGINATION", playerID=target, objectID=self }
 
 	--local driver = target:GetPossessor().possessorID
	--driver:UpdatePlayerStatistic{ updateID = 21 }
        
    --target:CastSkill{skillID = localskillID, optionalTargetID = target}
    --self:Die{ killerID = msg.playerID, killType = "SILENT" }
    --self:SetVar("bIsDead", true)
    
end