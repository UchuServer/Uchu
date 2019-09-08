require('o_mis')

function onTimerDone(self, msg)

	if msg.name == "RightDeathTeleport" then
        getObjectByName(self,"DrownRighter"):SetPosition {pos = {x = -106.02,y = 189.99,z = -561.75}}
        getObjectByName(self,"DrownRighter"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        storeObjectByName(self, "DrownRighter", target)
        getObjectByName(self,"DrownRighter"):SetUserCtrlCompPause{bPaused = true}
        getObjectByName(self,"DrownRighter"):PlayAnimation{animationID = "drown"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "RightDeathTeleport",self )
    end
end

