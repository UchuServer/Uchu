require('o_mis')

function onStartup(self, msg)

    self:AddObjectToGroup{ group = "MR_TrapPlace" }

end

function onUse(self,msg)

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        friends[i]:NotifyObject{name = "trapplaced", ObjIDSender = self}
    end

end

function onNotifyObject(self, msg)

    if msg.name == "delete" then
        GAMEOBJ:DeleteObject(self)
    end

end
