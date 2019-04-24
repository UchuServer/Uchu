require('o_mis')

function onStartup(self)

end

function onUse(self, msg)
	self:PlayFXEffect{effectType = "interact"}
end