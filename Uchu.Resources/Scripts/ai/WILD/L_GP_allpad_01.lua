require('o_mis')

function onStartup(self, msg)

    local ControlBlockerObject = self:GetObjectsInGroup{ group = "GP_ALL"}.objects[1]

    if ControlBlockerObject then
        storeObjectByName(self, "ControlBlockerObject", ControlBlockerObject)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "BlockerNotify",self )
    else
        print "Yegge rules"
    end
end

function onTimerDone(self, msg)

	if msg.name == "BlockerNotify" then
        getObjectByName(self,"ControlBlockerObject"):NotifyObject{name = "totalloaded", ObjIDSender = self}
    end

end 