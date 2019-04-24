--client-side switch

function onStartup(self)

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--interactive
	return msg

end