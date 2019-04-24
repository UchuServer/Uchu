function onClientUse(self)

    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "RoadFreezeandOrient",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.9, "RoadSnap",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "RoadUnfreezeandCamera",self )

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

function onTimerDone (self, msg)

	if (msg.name == "RoadFreezeandOrient") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = true}
        player:SetRotation {x=0,y=0,z=0,w=1}
    end

	if (msg.name == "RoadSnap") then
        CAMERA:SnapCameraToPlayer()
    end    
    
	if (msg.name == "RoadUnfreezeandCamera") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:PlayCinematic { pathName = "roadView" }
        player:SetUserCtrlCompPause{bPaused = false}
    end

end