require('o_mis')

function onClientUse(self)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    player:PlayCinematic { pathName = "roadView" }

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end
