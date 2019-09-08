require('o_mis')

function onTimerDone(self, msg)

    local vinepos = self:GetPosition{}.pos
    local vineleftx = vinepos.x - 9
    local vinerightx = vinepos.x + 9
    local viney = vinepos.y - 10
    local vinez = vinepos.z + 3
    local platformx = vinepos.x
    local platformy = vinepos.y + -11.545
    local platformz = vinepos.z + 1.859

    
	if msg.name == "Swing Left" then
        getObjectByName(self,"Tarzan"):SetPosition{pos = {x = vineleftx, y = viney, z = vinez}}
        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-hack"}
        getObjectByName(self,"Tarzan"):SetUserCtrlCompPause{bPaused = false}
    end
        
	if msg.name == "Swing Right" then
        getObjectByName(self,"Tarzan"):SetPosition{pos = {x = vinerightx, y = viney, z = vinez}}
        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-hack"}
        getObjectByName(self,"Tarzan"):SetUserCtrlCompPause{bPaused = false}
    end

	if msg.name == "Swing Teleport Left" then
        getObjectByName(self,"Tarzan"):SetUserCtrlCompPause{bPaused = true}
--        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-rt"}
        getObjectByName(self,"Tarzan"):SetPosition{pos = {x = platformx, y = platformy, z = platformz}}
        getObjectByName(self,"Tarzan"):SetRotation {x=0,y=-0.707,z=0,w=0.707}
    end

   	if msg.name == "Tele Hack Left" then
        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-lt"}
        self:PlayAnimation{animationID = "swing-lt"}
    end
    
	if msg.name == "Swing Teleport Right" then
        getObjectByName(self,"Tarzan"):SetUserCtrlCompPause{bPaused = true}
--        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-rt"}
        getObjectByName(self,"Tarzan"):SetPosition{pos = {x = platformx, y = platformy, z = platformz}}
        getObjectByName(self,"Tarzan"):SetRotation {x=0,y=0.707,z=0,w=0.707}
    end

   	if msg.name == "Tele Hack Right" then
        getObjectByName(self,"Tarzan"):PlayAnimation{animationID = "swing-rt"}
        self:PlayAnimation{animationID = "swing-rt"}
    end

	if msg.name == "Unfreeze" then -- To hack the animation freezing problem when two animations are toggled too quickly
        self:PlayAnimation{animationID = "idle"}
        print "You can swing now"
    end
end 

function onCollisionPhantom(self, msg)

    local vinepos = self:GetPosition{}.pos
    local target = msg.senderID
    local mypos = target:GetPosition().pos

    local vinepadx = vinepos.x
    local vinepady = vinepos.y
    local vinepadz = vinepos.z

    storeObjectByName(self, "Tarzan", target)


    if mypos.x > vinepos.x then

        --target:SetPosition {pos = {x = vinepadx, y = vinepady, z = vinepadz}}
        --target:SetUserCtrlCompPause{bPaused = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Swing Teleport Left",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.0, "Tele Hack Left",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "Swing Left",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 4.6, "Unfreeze",self )       
    elseif mypos.x < vinepos.x then
        --target:SetPosition {pos = vinepos}
        --target:SetUserCtrlCompPause{bPaused = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Swing Teleport Right",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.0, "Tele Hack Right",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "Swing Right",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 4.6, "Unfreeze",self )

    end

end
