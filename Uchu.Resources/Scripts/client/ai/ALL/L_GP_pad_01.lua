require ('o_mis')

function onRenderComponentReady(self, msg)

    print "pad starting up"

    local ControlObject = self:GetObjectsInGroup{ group = "GP_Control"}.objects[1]

    print (tostring(ControlObject:GetLOT().objtemplate))

    ControlObject:NotifyObject{name = "padloaded", ObjIDSender = self}
    storeObjectByName(self, "ControlObject", ControlObject)

end

--[[function onClientUse(self, msg)

    print "Click A Green Piece to Move It"

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end
--]]