require('o_mis')

function onStartup(self, msg)

    local ControlAllObject = self:GetObjectsInGroup{ group = "GP_ALL"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "AllNotify",self )
    storeObjectByName(self, "ControlAllObject", ControlAllObject)

end

function onTimerDone(self, msg)

	if msg.name == "AllNotify" then
        getObjectByName(self,"ControlAllObject"):NotifyObject{name = "totalloaded", ObjIDSender = self}
    end

end 

function onCollisionPhantom(self, msg)

    local target = msg.objectID
    local playerVelx = target:GetLinearVelocity().linVelocity.x / 2
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z / 2
    --target:PlayAnimation{animationID = "floatpad"}

    msg.objectID:Knockback{vector={x=0, y=40,z=0}}

end

function onOffCollisionPhantom(self, msg)

    local target = msg.objectID
    local playerVelx = target:GetLinearVelocity().linVelocity.x
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z

    msg.objectID:Knockback{vector={x=playerVelx, y=20,z=playerVelz}}

end
