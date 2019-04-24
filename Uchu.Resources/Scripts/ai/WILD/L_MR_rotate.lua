require('o_mis')

function onUse(self)

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        friends[i]:NotifyObject{name = "rotate", ObjIDSender = self}
    end

end
