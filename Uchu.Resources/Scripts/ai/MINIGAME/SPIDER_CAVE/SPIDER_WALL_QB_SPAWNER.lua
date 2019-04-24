--------------------------------------------------
--script to attach to the spider wall that spawns a quick build when destroyed
--------------------------------------------------

function onStartup (self, msg)
    --print("QB wall starting up!")
    --self:SetFaction{faction = 6}
end

function onDie (self, msg)
    if msg.killerID:GetID() ~= "0" then
        --print("QB wall down!")
        local mypos = self:GetPosition().pos
        local posString = self:CreatePositionString{ x = mypos.x - 15, y = mypos.y, z = mypos.z - 10}.string
        local myRot = self:GetRotation()
        local parent = msg.killerID;
        local config = { {"rebuild_activators", posString }, {"respawn", 100000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", false}--[[, {"CheckPrecondition" , "0:21"}--]] }
        --print("spawning QB")
        RESMGR:LoadObject { objectTemplate = 6483, x= mypos.x, y= mypos.y , z= mypos.z, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z, configData = config, owner = parent }
    end
end