require('o_mis')


function onTimerDone(self, msg)

	if msg.name == "LeftDeathTeleport" then
        getObjectByName(self,"WaterLeftPlayer"):SetPosition {pos = {x=-186,y=184.28,z=-551}}
        getObjectByName(self,"WaterLeftPlayer"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.senderID
    storeObjectByName(self, "WaterLeftPlayer", target)

    target:SetUserCtrlCompPause{bPaused = true}
    target:PlayAnimation{animationID = "chicken"}

	GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "LeftDeathTeleport",self )

end
