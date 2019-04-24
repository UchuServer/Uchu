require('equipmenttriggers/skillSetTriggerTemplate')

function onStartup(self)
	--conditions for firing the trigger
	self:SetVar("trigger", {Name="Low Armor", Stat="ARMOR", Operator="LESS", Value=1} )
	--skill to fire
	self:SetVar("skillID", 1103)
	--how many items from a given set they must have equipped before the skill fires.
	self:SetVar("itemsRequired", 4)
	--ID of the skill set they have to have equipped
	self:SetVar("skillSet", 12)
end
