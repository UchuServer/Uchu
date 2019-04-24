require('o_mis')

function onStartup(self, msg)

    local ControlBlockerObject = self:GetObjectsInGroup{ group = "GP_Blocker"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "BlockerNotify",self )
    storeObjectByName(self, "ControlBlockerObject", ControlBlockerObject)

end

function onTimerDone(self, msg)

	if msg.name == "BlockerNotify" then
        getObjectByName(self,"ControlBlockerObject"):NotifyObject{name = "blockerloaded", ObjIDSender = self}
    end

end 