function onClientUse(self)

    local ControlObject = self:GetObjectsInGroup{ group = "GP_Control"}.objects[1]
    ControlObject:NotifyObject{ObjIDSender=self, name = "Selected"}

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end