function onChoicebuildComplete(self, msg)    
    GAMEOBJ:GetTimer():AddTimerWithCancel(0.1, "resetUI", self )
end

--timers...
function onTimerDone(self, msg)
    if msg.name == "resetUI" then    
        GAMEOBJ:GetZoneControlID():FireEvent{senderID = self, args = "DeactivateRewards"}
        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SetGameState"}
        self:Die{killerID = self}
    end
end
        