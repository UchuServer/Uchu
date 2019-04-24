-----------------------------------------------------------
-- this script is on the cloud damaging fx in the property pushback maps
-- 
-- THIS SCRIPT NEEDS COMMENTING
-- brandi 6/3/10
--------------------------------

function onCollisionPhantom(self,msg)
	
	if msg.objectID:BelongsToFaction{factionID = 1}.bIsInFaction then
	
		self:CastSkill{skillID = 797, optionalTargetID = msg.objectID }

	end

end