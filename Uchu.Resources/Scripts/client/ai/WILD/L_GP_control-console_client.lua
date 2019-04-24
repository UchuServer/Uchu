--require('o_mis')

function onClientUse(self)

    --print "Start timers"
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PlayerTeleportFreezeandOrient",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.9, "Snap",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "PlayerUnfreezeandCamera",self )

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

function onTimerDone (self, msg)

	if (msg.name == "PlayerTeleportFreezeandOrient") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = true}
        --player:SetPosition {pos = {x=-144.16,y=184.28,z=-461.66}}
        player:SetRotation {x=0,y=-1,z=0,w=0}
    end

	if (msg.name == "Snap") then
        CAMERA:SnapCameraToPlayer()
    end    
    
	if (msg.name == "PlayerUnfreezeandCamera") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:RemoveSkill{ skillID = 66 }
        player:AddSkill{ skillID = 66 }
        player:CastSkill{ optionalTargetID = self, skillID = 66 }
        player:PlayCinematic { pathName = "Play" }
        player:SetUserCtrlCompPause{bPaused = false}
    end

end
