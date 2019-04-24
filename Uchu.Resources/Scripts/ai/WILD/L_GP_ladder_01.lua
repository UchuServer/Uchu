--[[require('o_mis')

function onStartup(self, msg)

    print "pad starting up"

    local ControlObject = self:GetObjectsInGroup{ group = "GP_Control"}.objects[1]

    print (tostring(ControlObject:GetLOT().objtemplate))

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "Notify",self )
    storeObjectByName(self, "ControlObject", ControlObject)

end

function onTimerDone(self, msg)

	if msg.name == "Notify" then
        getObjectByName(self,"ControlObject"):NotifyObject{name = "padloaded", ObjIDSender = self}
    end

end 

function onCollisionPhantom(self, msg)

    local player = msg.objectID
    local bouncepos = self:GetPosition().pos 
    local mypos = player:GetPosition().pos

    print "Collide"

    if mypos.x > bouncepos.x then
        msg.objectID:Knockback{vector={x=-10,y=40,z=0}}

    elseif mypos.x < bouncepos.x then
        msg.objectID:Knockback{vector={x=10,y=40,z=0}}
    end

end
--]]

function onCollisionPhantom(self, msg)
print "hit"
    local target = msg.objectID
    local elast = 200.0
    local maximumSpeed = 300.0

    local vec = self:GetUpVector().niUpVector
    local vel = self:GetLinearVelocity().linVelocity
    local playerVel = target:GetLinearVelocity().linVelocity

    target:Deflect{direction = vec, velocity = vel, elasticity = elast, maxSpeed = maximumSpeed, playerVelocity = playerVel}

    print("Direction:" .. self:GetVar("vec"))

    return msg

end
