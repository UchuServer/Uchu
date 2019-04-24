--client-side bouncer switch

function onStartup(self)

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--NPC type
	return msg

end