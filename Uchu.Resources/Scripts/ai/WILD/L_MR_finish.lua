require('o_mis')

function onStartup(self, msg)
    
    self:AddObjectToGroup{ group = "MR_FinishLine" }
    print "Added to group"
    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects

    for i = 1, table.maxn (friends) do
        friends[i]:NotifyObject{name = "traplabel", ObjIDSender = self}
    end

end

function onUse(self)
    print "Delete"
    GAMEOBJ:DeleteObject(self)

end