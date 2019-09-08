require('o_mis')

function onClientUse(self)

    --print "Start timers"
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "TeleportFreezeOrient",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.9, "Snap",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "UnfreezeTeleportCamera",self )

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

function onTimerDone (self, msg)

	if (msg.name == "TeleportFreezeOrient") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:SetUserCtrlCompPause{bPaused = true}
        player:SetRotation {x=0,y=-1,z=0,w=0}
    end

	if (msg.name == "Snap") then
        CAMERA:SnapCameraToPlayer()
    end    
    
	if (msg.name == "UnfreezeTeleportCamera") then
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

        CAMERA:ActivateCamera("CAMERA_SIDE_SCROLLER")
        CAMERA:SetRenderCamera("CAMERA_SIDE_SCROLLER")

        player:SetPosition{pos = {x=-106.02, y=189.99, z=-561.75}}
        player:SetRotation {x=0,y=0.707,z=0,w=0.707}
        player:SetUserCtrlCompPause{bPaused = false}
        player:RemoveSkill{ skillID = 170 }
        player:AddSkill{ skillID = 170 }
        player:CastSkill{ optionalTargetID = self, skillID = 170 }

        local plane = self:GetObjectsInGroup{ group = "Level" }.objects

            for i = 1, table.maxn (plane) do      
                if plane[i]:GetLOT().objtemplate == 5850 then
                     plane[i]:NotifyObject{ name="Lower" }
                end              
            end  
    end

end
