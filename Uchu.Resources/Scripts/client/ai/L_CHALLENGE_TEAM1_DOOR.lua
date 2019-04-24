
function onStartup(self)
	--UI:SendChat{ChatString = "door:onStartup", ChatType = "LOCAL", Timestamp = 500}
	--GAMEOBJ:GetTimer():AddTimerWithCancel( 30, "OpenDoor", self )
end

function onHit(self, msg)
	--UI:SendChat{ChatString = "door:onHit", ChatType = "LOCAL", Timestamp = 500}
end

onTimerDone = function(self, msg)
	if (msg.name == "OpenDoor") then
		--UI:SendChat{ChatString = "door:OpenDoor", ChatType = "LOCAL", Timestamp = 500}
		--self:PlayAnimation{ animationID = "open" }
    end
end
