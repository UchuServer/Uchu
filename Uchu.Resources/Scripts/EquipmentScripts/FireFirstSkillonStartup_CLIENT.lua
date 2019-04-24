
--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

        for i, skill in ipairs(self:GetSkills().skills) do
            self:CastSkill{skillID = skill, bLocalCastOnly = true} 
        end
    
end
















