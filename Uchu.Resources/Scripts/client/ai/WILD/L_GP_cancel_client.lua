--require('o_mis')

function onClientUse(self)

--    print "Return cam to normal"

    	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayCinematic { pathName = "Camera_End" }

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end