require('o_mis')

function onStartup(self, msg)

    local ControlMiscObject = self:GetObjectsInGroup{ group = "GP_Misc"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "MiscNotify",self )
    storeObjectByName(self, "ControlMiscObject", ControlMiscObject)

end

function onTimerDone(self, msg)

	if msg.name == "MiscNotify" then
        getObjectByName(self,"ControlMiscObject"):NotifyObject{name = "miscloaded", ObjIDSender = self}
    end

end 

