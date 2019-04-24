local localskillID = 500

function onCollisionPhantom(self, msg)
	local target = msg.objectID
	local faction = target:GetFaction()
	
	-- If a player collided with me, then do our stuff
	if faction and (faction.faction == 1 or faction.faction == 101 or faction.faction == 100 or faction.faction == 2) then
	 	    self:PlayFXEffect{effectType = "pickup"}
			target:CastSkill{skillID = localskillID, optionalTargetID = target}
            self:Die{ killerID = msg.playerID, killType = "SILENT" }
	end       	
	-- ONly do this once
  return msg
end