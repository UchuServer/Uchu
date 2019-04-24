------------------------------------------------
--switch to fire the cannon in the first room
------------------------------------------------

function onStartup(self)
    print("starting up the laser switch!")
    self:SetVar("TickTime", 3)
end

function onFireEvent(self, msg)
    print("switch activated!!")
    local friends = self:GetObjectsInGroup{group = "laser1", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6626) then
            object:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
        end
    end
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("TickTime")  , "HitTime", self )
    local friends = self:GetObjectsInGroup{group = "Crate2", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6452) then
            object:Die{killerID = self, killType = "VIOLENT"}
        end
    end
end

function onTimerDone(self, msg)
    local friends = self:GetObjectsInGroup{group = "laser1", ignoreSpawners = true}.objects
    for i, object in pairs(friends) do
        if (object:GetLOT().objtemplate == 6626) then
            object:StopFXEffect{name = "moviespotlight"}
        end
    end
end