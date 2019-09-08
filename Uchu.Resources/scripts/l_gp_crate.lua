require('o_mis')

function onStartup(self, msg)

    local ControlMiscObject = self:GetObjectsInGroup{ group = "GP_Misc"}.objects[1]

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "MiscNotify",self )
    storeObjectByName(self, "ControlMiscObject", ControlMiscObject)

end

function onTimerDone(self, msg)

	if msg.name == "MiscNotify" then
        getObjectByName(self,"ControlMiscObject"):NotifyObject{name = "miscloaded", ObjIDSender = self}
    end

end 

function onCollisionPhantom(self, msg)

    local target = msg.senderID

    local playerVelx = target:GetLinearVelocity().linVelocity.x
    local playerVely = target:GetLinearVelocity().linVelocity.y
    local playerVelz = target:GetLinearVelocity().linVelocity.z
    local cratepos = self:GetPosition{}.pos
    local crateposx = cratepos.x + playerVelx / 10
    local crateposy = cratepos.y
    local crateposz = cratepos.z + playerVelz / 10
    target:PlayAnimation{animationID = "push-crate"}
    self:SetPosition{pos = {x = crateposx, y = crateposy, z = crateposz}}

end
