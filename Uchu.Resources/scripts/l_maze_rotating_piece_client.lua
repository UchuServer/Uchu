require('o_mis')
require('o_Main')


function onStartup(self)
end




function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--NPC type
	return msg

end



--function onClientUse(self, msg)
	-- this DOES rotate the object, but only for the client who clicked it
	--self:SetRotation{ x = 0.0, y = 0.7071, z = 0.0, w = 0.7071, bIgnoreDirtyFlags = true }
--end
