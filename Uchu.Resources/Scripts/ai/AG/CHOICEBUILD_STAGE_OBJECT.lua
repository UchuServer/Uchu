

function onChoicebuildComplete(self, msg)
	GAMEOBJ:GetZoneControlID():NotifyObject{name = "ChoicebuildChanged", param1 = msg.index, ObjIDSender = self}
end