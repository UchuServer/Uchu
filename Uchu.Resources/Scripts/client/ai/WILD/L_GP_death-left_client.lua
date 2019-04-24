require('o_mis')

function onTimerDone(self, msg)

	if msg.name == "LeftDeathTeleport" then
        getObjectByName(self,"DrownLefter"):SetPosition {pos = {x = -173.5,y = 190,z = -561.74}}
        getObjectByName(self,"DrownLefter"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        storeObjectByName(self, "DrownLefter", target)
        getObjectByName(self,"DrownLefter"):SetUserCtrlCompPause{bPaused = true}
        getObjectByName(self,"DrownLefter"):PlayAnimation{animationID = "drown"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "LeftDeathTeleport",self )
    end
    
end

