--------------------------------------------
-- Server Side script for the Rank 1 Daredevil First Aid Kit 
-- used to allow the player to click on the object to grant a heal and keeps track of heal "charges" it is allowed
--------------------------------------------



function onStartup(self) 

        GAMEOBJ:GetTimer():AddTimerWithCancel( 30  , "DieTime" , self )
        self:SetVar("charges", 4)

end

-------------------------
-- Check if the timer for self death is done.
-------------------------
onTimerDone = function(self, msg)
    if msg.name == "DieTime" then
         self:Die()
    end
   
end
-- when used, check if the player needs health and then cast the skill the kit has
function onUse(self, msg)

    if msg.user:GetHealth{}.health < msg.user:GetMaxHealth{}.health then
       local skill = self:GetSkills{}.skills
        self:CastSkill{skillID = skill[1], optionalTargetID = msg.user}
        self:SetVar("charges", self:GetVar("charges")-1)
        if self:GetVar("charges") < 1 then
            self:RequestDie()
        end
    end
end