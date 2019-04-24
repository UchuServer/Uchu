-- Script used to start and kill a skill on an object that will be randomly spawing through a spawner network.
local mySkill = 607

function onStartup(self, msg)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2  , "startSkill", self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 7.5  , "killSelf", self )
end


function onTimerDone(self, msg)
    if msg.name == "startSkill" then
        self:CastSkill{skillID = mySkill}
    end
    if msg.name == "killSelf" then
        self:RequestDie()
    end
end