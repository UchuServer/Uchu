--L_ACT_CANNONBALL.lua

require('o_mis')

local skillID = 47

-- cast the skill for the projectile
function onProjectileImpact(self, msg)
	self:CastSkill{skillID = skillID}
end

function onUpdateMissionTask(self, msg)
	--forward message to parent
	getParent(self):UpdateMissionTask{target = msg.target, value = msg.value, taskType = msg.taskType}
end
