--------------------------------------------------------------
-- (CLIENT SIDE) Script for meteors in scene 2
--
--------------------------------------------------------------

--------------------------------------------------------------
-- Called when an object hits the final waypoint on a path
--------------------------------------------------------------
function onPlatformAtLastWaypoint(self, msg)

	-- notify the zone object, pass our template id
	GAMEOBJ:GetZoneControlID():NotifyObject{ name = "meteor_path_complete", param1 = self:GetLOT().objtemplate }
	
	-- remove ourself
	GAMEOBJ:DeleteObject(self)

end
