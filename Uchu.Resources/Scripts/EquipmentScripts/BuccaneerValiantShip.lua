--------------------------------------------------------------
-- Makes the ship cast its skill 2 seconds after spawning
-- MEdwards 11/12/10
-- last Modified MEdwards 5-5-11
--------------------------------------------------------------
function onStartup(self) 
    -- give the object 1 second to "spawn in" before firing its skill
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "SkillCastTimer", self )
end

function onTimerDone(self, msg)
    -- After the timer is up, cast whatever skill the object has
    if msg.name == "SkillCastTimer" then
        self:CastSkill{skillID = 982, optionalOriginatorID = self:GetParentObj{}.objIDParent}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.1 , "selfSmashTimer", self )
    end
     if msg.name == "selfSmashTimer" then
       self:RequestDie()
    end
end
