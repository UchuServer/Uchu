function onStartup(self, msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 4.2, "KillRooster",self )

end

function onTimerDone(self, msg)

	if msg.name == "KillRooster" then
        self:Die{ killerID = self, killType = "SILENT" }
    end

end