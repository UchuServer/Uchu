require('o_mis')

function onStartup(self, msg)

    local ControlClimberObject = self:GetObjectsInGroup{ group = "GP_Climb"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "ClimberNotify",self )
    storeObjectByName(self, "ControlClimberObject", ControlClimberObject)

end

function onTimerDone(self, msg)

	if msg.name == "ClimberNotify" then
        getObjectByName(self,"ControlClimberObject"):NotifyObject{name = "climberloaded", ObjIDSender = self}
    end

end
