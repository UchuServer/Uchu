function onStartup(self,msg)

    self:SetVar("organuse", 0)

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end

function onClientUse(self,msg)

    local player = msg.user
    self:PlayAnimation{animationID = "down"}

    if self:GetVar("organuse") == 0 then
        print "Organ Time!"
        player:SetAnimationSet{strSet = "horse-mount", bPush=true}
        player:PlayCinematic { pathName = "OrganCam" }
        player:SetPosition{pos = {x = 635.356, y = 200, z = -142.295}}
        player:SetRotation{x=0, y=1, z=0, w=0}
        self:SetVar("organuse", 1)
    else
        print "Back to normal."
        player:SetAnimationSet{strSet = "", bPush=false}
        player:PlayCinematic { pathName = "CancelCam" }
        self:SetVar("organuse", 0)
    end

end
