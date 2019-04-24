
function onStartup(self)
	--UI:SendChat{ChatString = "bumper:onStartup", ChatType = "LOCAL", Timestamp = 500}

	-- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	--UI:SendChat{ChatString = "rock:onstartup", ChatType = "LOCAL", Timestamp = 500}

end
