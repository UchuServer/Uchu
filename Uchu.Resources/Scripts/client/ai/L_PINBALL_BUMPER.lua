
function onStartup(self)
	--UI:SendChat{ChatString = "bumper:onStartup", ChatType = "LOCAL", Timestamp = 500}

	-- register with zone control object
	GAMEOBJ:GetZoneControlID():ObjectLoaded{objectID = self, templateID = self:GetLOT().objtemplate}
end

function onCollision(self, msg)
	local target = GAMEOBJ:GetZoneControlID()

	-- Send the message to the zone object, who will relay it to C++
	target:ArcadeScoreEvent{objectID = self, templateID = self:GetLOT().objtemplate}
end
