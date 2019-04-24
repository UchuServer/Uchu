
function onCollisionPhantom(self,msg)
	
	if msg.objectID:BelongsToFaction{factionID = 1}.bIsInFaction then
	
		self:CastSkill{skillID = 692}

	end

end