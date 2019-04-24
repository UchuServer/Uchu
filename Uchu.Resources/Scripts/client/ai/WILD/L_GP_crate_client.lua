--require('o_mis')

function onCollisionPhantom(self, msg)

    print "teleport"

    local target = msg.senderID

    local playerVelx = target:GetLinearVelocity().linVelocity.x
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z

    local cratepos = self:GetPosition{}.pos
    local crateposx = cratepos.x + playerVelx / 5
    local crateposy = cratepos.y + playerVely / 5
    local crateposz = cratepos.z + playerVelz / 5

    self:SetPosition{pos = {x = crateposx, y = crateposy, z = crateposz}}
end

function onOffCollisionPhantom(self, msg)

    print "update leave"

end