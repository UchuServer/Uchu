require('o_mis')


function onStartup(self)
	self:SetRegistrationForUIUpdate{ eEventType = "POSITION_CHANGE", iMinimapObjType = 9, bRegister = true }
end
