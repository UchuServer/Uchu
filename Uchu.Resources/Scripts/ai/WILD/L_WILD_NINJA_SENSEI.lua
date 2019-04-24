function onStartup(self, msg)

    self:PlayAnimation{ animationID = "bow" }
	GAMEOBJ:GetTimer():AddTimerWithCancel( 6.5, "CraneStart",self )

end

function onTimerDone(self, msg)

	if msg.name == "CraneStart" then
        local Ninjas = self:GetObjectsInGroup{ group = "Ninjastuff"}.objects

        for i = 1, table.maxn (Ninjas) do
            Ninjas[i]:NotifyObject{ name="Crane" }
        end

        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.5, "Bow",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 25.0, "TigerStart",self )
        self:PlayAnimation{ animationID = "crane" }

    elseif msg.name == "TigerStart" then
        local Ninjas = self:GetObjectsInGroup{ group = "Ninjastuff"}.objects

        for i = 1, table.maxn (Ninjas) do
            Ninjas[i]:NotifyObject{ name="Tiger" }
        end

        GAMEOBJ:GetTimer():AddTimerWithCancel( 15.5, "Bow",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 25.0, "MantisStart",self )
        self:PlayAnimation{ animationID = "tiger" }

	elseif msg.name == "MantisStart" then
        local Ninjas = self:GetObjectsInGroup{ group = "Ninjastuff"}.objects

        for i = 1, table.maxn (Ninjas) do
            Ninjas[i]:NotifyObject{ name="Mantis" }
        end

        GAMEOBJ:GetTimer():AddTimerWithCancel( 16.5, "Bow",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 25.0, "CraneStart",self )
        self:PlayAnimation{ animationID = "mantis" }

    elseif msg.name == "Bow" then
        local Ninjas = self:GetObjectsInGroup{ group = "Ninjastuff"}.objects

        for i = 1, table.maxn (Ninjas) do
            Ninjas[i]:NotifyObject{ name="Bow" }
        end
        self:PlayAnimation{ animationID = "bow" }
    end

end