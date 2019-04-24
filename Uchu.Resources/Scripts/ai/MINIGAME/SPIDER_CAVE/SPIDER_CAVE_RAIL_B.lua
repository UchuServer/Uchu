function onStartup(self)
    --print("rail B starting up")
end

function onRebuildNotifyState(self, msg)
    if (msg.iState == 2) then
        local friends = self:GetObjectsInGroup{group = "SpiderRailTurret", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 6528) then
                object:NotifyObject{name = "lastPoint", ObjIDSender = self}
            end
        end
    end
end