function onCollisionPhantom(self, msg)
	GAMEOBJ:GetZoneControlID():NotifyObject{name = "BigCollectableCollected"}
	GAMEOBJ:DeleteObject(self)
	return msg
end