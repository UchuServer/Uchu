
function onFactionTriggerItemEquipped (self)
    self:AddStatTrigger(self:GetVar("trigger"))
end

function onStatEventTriggered(self, msg)
	if msg.Parent:Exists{} then 
		-- the value of the stat
		local statValue = msg.StatValue
		-- Fire the toughness skill
		if statValue ~= msg.OldValue then
			--IsItemInSet will give us a list of all the sets an item is in
			local setInfo = msg.Parent:IsItemInSet{setItem = self}
            if setInfo.setIDs then
                if self:GetVar("isFactionSkill") == true and self:GetVar("factionSkillReady") == true then
                    checkSetAndCast(self, setInfo.setIDs, msg.Parent)
                    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("factionCooldownTime") , "skillCooldown", self )
                    self:SetVar("factionSkillReady", false)
                elseif not self:GetVar("isFactionSkill") then
                    checkSetAndCast(self, setInfo.setIDs, msg.Parent)
                end
            end
		end
	end
end

function checkSetAndCast(self, set, itemParent)
	for index, skillSetID in ipairs(set) do
		--verify that we're firing from the right set
		if skillSetID == self:GetVar("skillSet") then
			--fetch all the currently equipped items from our set.
			local equippedItems = itemParent:GetEquippedItemsInSet{setID = skillSetID}
			if self:GetVar("itemsRequired") <= #equippedItems.setItems then
				--if we are the first item in the list we're going to cast the skill.
				--All the items in the set will get this far, but only one of them will have to
				--actually fire the skill
				local selfID = self:GetID()
				local equipSet = equippedItems.setItems
				local items1 = equipSet[1]
				local itemsID = items1:GetID()				
				
				if selfID == itemsID then
					itemParent:CastSkill{ skillID = self:GetVar("skillID") }
					return
				end
			end
		end
	end
end

function onTimerDone(self, msg)
    if msg.name == "skillCooldown" then
        self:SetVar("factionSkillReady", true)
    end
end