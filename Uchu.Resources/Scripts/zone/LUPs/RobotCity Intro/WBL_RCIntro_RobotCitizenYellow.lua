function onStartup(self)
   	self:FollowWaypoints{bPaused = false}
end

function onUse(self, msg)

	local clicker = msg.user
    self:FaceTarget{target = clicker, degreesOff = 0, keepFacingTarget = false, bInstant = true}
    self:PlayAnimation{ animationID = "wave" }
    local animtime = self:GetAnimationTime{ animationID = "wave" }.time
    GAMEOBJ:GetTimer():AddTimerWithCancel(animtime, "animation time", self)
end

function onTimerDone(self, msg)
    self:FollowWaypoints{bPaused = false}
end