require('o_mis')


function onTimerDone(self, msg)

	if msg.name == "RightDeathTeleport" then
        getObjectByName(self,"WaterRightPlayer"):SetPosition{pos = {x=106.02, y=189.99, z=-561.75}}
        getObjectByName(self,"WaterRightPlayer"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.senderID
    storeObjectByName(self, "WaterRightPlayer", target)

    target:SetUserCtrlCompPause{bPaused = true}
    target:PlayAnimation{animationID = "chicken"}

	GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "RightDeathTeleport",self )

end