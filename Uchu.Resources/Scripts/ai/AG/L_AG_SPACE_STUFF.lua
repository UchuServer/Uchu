function onStartup(self, msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel(5.0, "FloaterScale", self)

end

function onTimerDone(self, msg)

	if msg.name == "FloaterScale" then
        -- Type of objects called
        local scaletype = math.random(1,5)

        --print ("Scale " .. scaletype)

        self:PlayAnimation{animationID = "scale_0" .. scaletype}

        GAMEOBJ:GetTimer():AddTimerWithCancel(0.4, "FloaterPath", self)

	elseif msg.name == "FloaterPath" then

        local pathtype = math.random(1,4)
        local randtime = math.random(20,25)

        --print ("Move " .. pathtype)

        self:PlayAnimation{animationID = "path_0" .. pathtype}

        GAMEOBJ:GetTimer():AddTimerWithCancel(randtime, "FloaterScale", self)

    end

end