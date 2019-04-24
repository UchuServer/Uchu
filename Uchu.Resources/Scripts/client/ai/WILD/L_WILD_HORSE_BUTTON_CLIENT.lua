function onStartup(self,msg)

    self:SetVar("horse", 0)

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end

function onClientUse(self,msg)

    local player = msg.user

    self:PlayAnimation{animationID = "down"}

    if self:GetVar("horse") == 0 then
        print "Horse time!"
        player:SetAnimationSet{strSet = "horse-mount", bPush=true}
        player:PlayAnimation{ animationID = "mount-horse-mount" }
        self:SetVar("horse", 1)
    else
        print "Back to normal."
        player:SetAnimationSet{strSet = "horse-mount", bPush=false}
        self:SetVar("horse", 0)
    end

end