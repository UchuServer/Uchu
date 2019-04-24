local localskillID = 80

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    local target = msg.objectID
    
    -- If a player collided with me, then cast the skill on him
    if target:BelongsToFaction{factionID = 1}.bIsInFaction or target:BelongsToFaction{factionID = 101}.bIsInFaction then
        if ( (target:GetArmor().armor) < (target:GetMaxArmor().armor) ) then
            self:PlayFXEffect{effectType = "pickup"}
            self:CastSkill{skillID = localskillID, optionalTargetID = target}
            self:Die{ killerID = msg.playerID, killType = "SILENT" }
            self:SetVar("bIsDead", true)
        end
    end
end