function onStartup(self, msg)

    self:SetVar("flipped", 1)
    self:PlayAnimation{ animationID = "idle" }

end

function onUse(self, msg)

    if self:GetVar("flipped") == 1 then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.6, "Flipping",self )
        self:PlayAnimation{ animationID = "flip-over" }
        self:SetVar("flipped", 0)

    elseif self:GetVar("flipped") == 0 then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.8, "Flipback",self )
        self:PlayAnimation{ animationID = "flip-back" }
        self:SetVar("flipped", 1)
    end

end

function onTimerDone(self, msg)

	if msg.name == "Flipping" then
        self:PlayAnimation{ animationID = "over-idle" }

    elseif msg.name == "Flipback" then
        self:PlayAnimation{ animationID = "idle" }
    end

end 