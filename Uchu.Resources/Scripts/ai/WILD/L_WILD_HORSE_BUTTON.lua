function onStartup(self,msg)

    self:SetVar("horse", 0)

end

function onUse(self, msg)
    local player = msg.user
    local item = player:AddNewItemToInventory{ iObjTemplate = 7089 }

    if self:GetVar("horse") == 0 then
		player:EquipInventory{ itemtoequip = item.newObjID }
        self:SetVar("horse", 1)
    else
        player:RemoveItemFromInventory{ iObjTemplate = 7089 }
        self:SetVar("horse", 0)
    end

end
