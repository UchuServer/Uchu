function Initialize(self)

  self:SetRegistrationForUIUpdate{ eEventType = "OVERHEAD_ICON_CHANGE", iMinimapObjType = 5 }

end

--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 9
	return msg

end