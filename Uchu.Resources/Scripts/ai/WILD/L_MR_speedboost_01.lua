require('o_mis')

function onStartup(self, msg)

    local friends = self:GetObjectsInGroup{ group = "MR_Control"}.objects

    for i = 1, table.maxn (friends) do
        friends[i]:NotifyObject{name = "traplabel", ObjIDSender = self}
    end

end

function onUse(self)

    GAMEOBJ:DeleteObject(self)

end

function onCollisionPhantom(self, msg)

    if self:GetRotation{}.y == 1 then
        msg.objectID:Knockback{vector={x = 0, y = 0, z = -100}}

    elseif self:GetRotation{}.y > 0.7 and msg.objectID:GetRotation{}.y < 0.8 then
        msg.objectID:Knockback{vector={x = 100, y = 0, z = 0}}

    elseif self:GetRotation{}.y == 0 then
        msg.objectID:Knockback{vector={x = 0, y = 0, z = 100}}

    elseif self:GetRotation{}.y < -0.7 and msg.objectID:GetRotation{}.y > -0.8 then
        msg.objectID:Knockback{vector={x = -100, y = 0, z = 0}}
    end

    msg.objectID:PlayAnimation{animationID = "turbo-racecar"}

end