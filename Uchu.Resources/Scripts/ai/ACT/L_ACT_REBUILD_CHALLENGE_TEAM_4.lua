-- Called anytime the rebuild object's state changes
-- We use this to notify the zone that we've completed out rebuild
function onRebuildNotifyState(self, msg)
    -- if we just hit the idle state
	if (msg.iState == 3) then
		GAMEOBJ:GetZoneControlID():UpdateMissionTask{taskType = "rebuildcomplete" .. self:GetLOT().objtemplate, target = self}
	end
end
