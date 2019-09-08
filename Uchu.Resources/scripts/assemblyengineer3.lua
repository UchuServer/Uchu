require('skillSetTriggerTemplate')

function onStartup(self)
	--conditions for firing the trigger
	self:SetVar("trigger", {Name="Low Imagination", Stat="IMAGINATION", Operator="LESS", Value=1} )
	--skill to fire
	self:SetVar("skillID", 582)
	--how many items from a given set they must have equipped before the skill fires.
	self:SetVar("itemsRequired", 4)
	--ID of the skill set they have to have equipped
	self:SetVar("skillSet", 4)
	-- Confirms that this is a faction set skill trigger
	self:SetVar("isFactionSkill", true)
	-- Set the faction skill as ready to fire
	self:SetVar("factionSkillReady", true)
	-- The time the faction skill has for its "cooldown"
	self:SetVar("factionCooldownTime", 11)
end
