--L_ENEMY_FIREBALL.lua

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- cast the skill for the projectile
--------------------------------------------------------------
function onProjectileImpact(self, msg)

	skill = self:GetVar("ImpactSkillID")
	if (skill) then
		self:CastSkill{skillID = skill}
	end
end


--------------------------------------------------------------
-- return the parent's faction
--------------------------------------------------------------
function onGetFaction(self, msg)
	msg.faction = self:GetVar("My_Faction")
end


--------------------------------------------------------------
-- Determine if the target is an enemy
--------------------------------------------------------------
function onIsEnemy(self, msg)
	-- get our faction from our parent
	local myFaction = self:GetVar("My_Faction")

	-- get the target's faction
	local tgt = msg.targetID;
	local tgtFaction = tgt:GetFaction().faction

	-- target is an enemy if the faction is not the same as us
	msg.enemy = (myFaction ~= tgtFaction)
	return msg
end


--------------------------------------------------------------
-- We get this message when the projectile kills something
--------------------------------------------------------------
--function onUpdateMissionTask(self, msg)
--	--forward message to parent
--	getParent(self):UpdateMissionTask{target = msg.target, value = msg.value, taskType = msg.taskType}
--end


--------------------------------------------------------------
-- Ignore collisions with our same faction
--------------------------------------------------------------
function onCollision(self, msg)
	
	local target = msg.objectID
	
	-- if we want to check for player status:
	local faction = target:GetFaction()
	if faction and faction.faction == self:GetVar("My_Faction") then
	
		msg.ignoreCollision = true
		
	else
	
		msg.ignoreCollision = false
		
	end
	
	return msg
	
end
