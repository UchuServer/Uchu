require('o_mis')

function onStartup(self)

	GAMEOBJ:GetZoneControlID():NotifyObject{ name="Blue_Point", ObjIDSender = self }

end