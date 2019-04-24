require('o_mis')

function onUse(self)

    local friends = self:GetObjectsInGroup{ group = "MR_Control" }.objects

    for i = 1, table.maxn (friends) do 
        friends[i]:NotifyObject{name = "up", ObjIDSender = self}
    end

    local roads = self:GetObjectsInGroup{ group = "MR_Roads" }.objects

    for i = 1, table.maxn (roads) do 
        roads[i]:NotifyObject{name = "up", ObjIDSender = self}
    end

end
