require('o_mis')

function onStartup(self, msg)

    local ControlAllObject = self:GetObjectsInGroup{ group = "GP_ALL"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "AllNotify",self )
    storeObjectByName(self, "ControlAllObject", ControlAllObject)

end

function onTimerDone(self, msg)

	if msg.name == "AllNotify" then
        getObjectByName(self,"ControlAllObject"):NotifyObject{name = "totalloaded", ObjIDSender = self}
    end

end 
