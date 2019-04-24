----------------------------------------
-- Generic Client side script that displays AI on the mini-map
--
-- Created by abeechler... 2/1/11 - shared script for mobs
----------------------------------------

function onStartup(self)
	self:SetRegistrationForUIUpdate{ eEventType = "POSITION_CHANGE", iMinimapObjType = 9, bRegister = true }
end
