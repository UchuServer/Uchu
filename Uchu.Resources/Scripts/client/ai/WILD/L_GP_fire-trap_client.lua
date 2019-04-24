require('o_mis')

function onTimerDone(self, msg)

	if msg.name == "FireDeathTeleport" then
        getObjectByName(self,"Burner"):SetPosition{pos = {x = -106.02,y = 189.99,z = -561.75}}
        getObjectByName(self,"Burner"):SetUserCtrlCompPause{bPaused = false}
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()
    
    if faction and faction.faction == 1 then
        storeObjectByName(self, "Burner", target)
        getObjectByName(self,"Burner"):SetUserCtrlCompPause{bPaused = true}
        getObjectByName(self,"Burner"):PlayAnimation{animationID = "burn"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "FireDeathTeleport",self )
    end
end

