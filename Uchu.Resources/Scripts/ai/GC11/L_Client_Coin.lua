--~ function onCollision(self, msg)
--~ 	local target = GAMEOBJ:GetZoneControlID()

--~ 	self:KillObj{targetID = self}	-- Send the message to the zone object, who will relay it to C++
--~ 	target:ArcadeScoreEvent{objectID = self}
--~ end
