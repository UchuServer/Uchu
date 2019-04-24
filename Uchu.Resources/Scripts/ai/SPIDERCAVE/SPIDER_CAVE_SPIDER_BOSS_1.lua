function onStartup(self) 

    self:SetVar("HitTime", 3)
    local group = self:GetObjectsInGroup{group = "SpiderWall", ignoreSpawners = true}.objects
    for i, object in pairs(group) do
        if (object:GetLOT().objtemplate == 6463) then
            print("setting wall")
            self:AddThreatRating{newThreatObjID = object, ThreatToAdd = -1000}
        end
    end
end

function onNotifyObject(self, msg)
    --print("spider got a message!")
    if msg.name == "shocked" then
        self:PlayAnimation{animationID = "spider-electrocute"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("HitTime")  , "HitTime", self )
    end
end

function onTimerDone(self, msg)
    self:Die()
end