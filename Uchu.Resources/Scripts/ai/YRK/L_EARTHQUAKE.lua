


function onStartup(self) 

	-- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
	
end