require('o_mis')

function onUse(self, msg)

    local placers = self:GetObjectsInGroup{ group = "MR_TrapPlace" }.objects

    for i = 1, table.maxn (placers) do 
        placers[i]:NotifyObject{name = "delete", ObjIDSender = self}
    end

end
