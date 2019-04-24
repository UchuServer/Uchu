
-----------------------------------------------------------
-- Radar Dish Rebuild Effects
-- Updated 3/15 Darren McKinsey
-----------------------------------------------------------

--self:SetRebuildState{iState = 0}


function onRebuildComplete(self,msg)
	print("called rebuild")
	self:PlayFXEffect{name = "radarDish", effectType = "create", effectID = 641}
end

