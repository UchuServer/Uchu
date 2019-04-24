function onCollisionPhantom(self, msg)
	GAMEOBJ:GetZoneControlID():NotifyObject{name = "CollectableCollected"}
	GAMEOBJ:DeleteObject(self)
	return msg
end