
function onFireEvent(self, msg)
    if ( msg.args == "fireball1") then
         self:CastSkill{skillID = 762}
    end
end
