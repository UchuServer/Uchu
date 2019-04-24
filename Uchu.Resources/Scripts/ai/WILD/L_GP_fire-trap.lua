require('o_mis')

function onStartup(self, msg)

    local ControlTrapObject = self:GetObjectsInGroup{ group = "GP_Trap"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "TrapNotify",self )
    storeObjectByName(self, "ControlTrapObject", ControlTrapObject)

end

function onTimerDone(self, msg)

	if msg.name == "TrapNotify" then
        getObjectByName(self,"ControlTrapObject"):NotifyObject{name = "traploaded", ObjIDSender = self}
    
    end
end
