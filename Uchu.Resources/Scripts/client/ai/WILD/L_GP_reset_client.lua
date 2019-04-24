function onClientUse(self)

    	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

        CAMERA:SetToPrevGameCam()

        player:RemoveSkill{ skillID = 170 }
        player:AddSkill{ skillID = 174 }
        player:CastSkill{ optionalTargetID = self, skillID = 174 }

end

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end