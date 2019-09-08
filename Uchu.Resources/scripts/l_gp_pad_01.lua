require('o_mis')

function onStartup(self, msg)

    local ControlObject = self:GetObjectsInGroup{ group = "GP_Pad"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "PadNotify",self )
    storeObjectByName(self, "ControlObject", ControlObject)

end

function onTimerDone(self, msg)

	if msg.name == "PadNotify" then
        getObjectByName(self,"ControlObject"):NotifyObject{name = "padloaded", ObjIDSender = self}
    end

end 
