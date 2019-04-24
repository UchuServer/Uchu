--------------------------------------------------
--script on the QB wall that spawns another quick build when destroyed and resets it's faction
--------------------------------------------------

function onStartup (self, msg)
    --print("QB wall starting up!")
    self:SetFaction{faction = 6}
    --print("wall faction set to 6!")
end

function onRebuildNotifyState(self, msg)
    --print("notifying state")
    if (msg.iState == 2) then
        self:SetFaction{faction = 37}
        --print("faction set to 37")
    elseif (msg.iState == 0) then
        self:SetFaction{faction = 6}
        --print("faction set to 6")
    end
end

function onDie (self, msg)
    if msg.killerID:GetID() ~= "0" then
        --print("QB wall down!")
        local mypos = self:GetPosition().pos
        local posString = self:CreatePositionString{ x = mypos.x - 10, y = mypos.y, z = mypos.z - 20 }.string
        local myRot = self:GetRotation()
        local parent = msg.killerID;
        local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", true}, {"CheckPrecondition" , "0:21"} }
        --print("spawning QB")
        RESMGR:LoadObject { objectTemplate = 6483, x= mypos.x, y= mypos.y , z= mypos.z, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config, owner = parent }
    end
end