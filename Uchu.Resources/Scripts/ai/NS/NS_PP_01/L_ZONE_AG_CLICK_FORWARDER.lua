-- This script is used by the modular build "ghost" pieces, to make them start modular build when they're clicked
-- It also forwards object drop messages to the parent modular build so you can drag parts into place from your inventory

local myPriority = 0.8

function onClientUse(self, msg)
	local propertyPlaques = self:GetObjectsInGroup{ group = "PropertyPlaque", ignoreSpawners = true }.objects
	--local propertyPlaques = GAMEOBJ:GetObjectsByLOT(3315)	
	for i = 1, table.maxn(propertyPlaques) do
		propertyPlaques[i]:PropertyEditIconInteraction{}
	end
end
