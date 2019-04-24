function onStartup(self)
    self:AddObjectToGroup{ group = "MR_FinishLine" }
end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end
