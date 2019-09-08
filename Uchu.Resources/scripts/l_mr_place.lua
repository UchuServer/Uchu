require('o_mis')

function onStartup(self, msg)

    local ControlObject = self:GetObjectsInGroup{ group = "MR_Control"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "PadNotify",self )
    storeObjectByName(self, "ControlObject", ControlObject)

end

function onTimerDone(self, msg)

	if msg.name == "PadNotify" then
        getObjectByName(self,"ControlObject"):NotifyObject{name = "roadloaded", ObjIDSender = self}
    end

end 


function onUse(self)

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        friends[i]:NotifyObject{name = "placed", ObjIDSender = self}
    end

end
