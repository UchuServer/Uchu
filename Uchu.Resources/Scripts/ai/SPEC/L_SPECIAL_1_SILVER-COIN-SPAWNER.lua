--L_SPECIAL_SILVER-COIN-SPAWNER.lua

local newcurrency = 0

function onCollisionPhantom(self, msg)
	local target = msg.objectID
	
	if target:BelongsToFaction{factionID = 1}.bIsInFaction then
            self:PlayFXEffect{effectType = "pickup"}
			newcurrency = target:GetCurrency().currency
            newcurrency = newcurrency + 100
            target:SetCurrency {currency = newcurrency}
            self:Die{ killerID = msg.playerID, killType = "SILENT" }
	end       	
  return msg
end

function onHasBeenCollected(self, msg)
	local target = msg.playerID
	
	if target:BelongsToFaction{factionID = 1}.bIsInFaction then
			local newcurrency = target:GetCurrency().currency
            newcurrency = newcurrency + 100
            target:SetCurrency {currency = newcurrency}
	end       	
  return msg
end