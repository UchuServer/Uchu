require('o_mis')

function onStartup(self)

	GAMEOBJ:GetZoneControlID():NotifyObject{ name="Red_Mark", ObjIDSender = self }

end
