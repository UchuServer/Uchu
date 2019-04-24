require('o_mis')

function onTimerDone(self, msg)

	if msg.name == "SpikeDeathTeleport" then
        getObjectByName(self,"Spiker"):SetPosition{pos = {x = -106.02,y = 189.99,z = -561.75}}
        getObjectByName(self,"Spiker"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()
    
    if faction and faction.faction == 1 then
        storeObjectByName(self, "Spiker", target)
        getObjectByName(self,"Spiker"):SetUserCtrlCompPause{bPaused = true}
        getObjectByName(self,"Spiker"):PlayAnimation{animationID = "shocked"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "SpikeDeathTeleport",self )
    end    
end

