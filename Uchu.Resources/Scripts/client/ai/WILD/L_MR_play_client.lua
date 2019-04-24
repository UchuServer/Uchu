function onClientUse(self)

    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

    player:AddSkill{ skillID = 66 }
    player:CastSkill{ optionalTargetID = self, skillID = 66 }
    player:SetAnimationSet{strSet = "racecar"}
    player:PlayCinematic { pathName = "Camera_End" }
    CAMERA:SnapCameraToPlayer()

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end