require('o_mis')

function onCollisionPhantom(self, msg)

    local target = msg.objectID

    local playerVelx = target:GetLinearVelocity().linVelocity.x
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z

    msg.objectID:Knockback{vector={x=playerVelx, y=30,z=playerVelz}}


end
