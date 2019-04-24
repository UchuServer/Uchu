--///////////////////////////////////////////////////////////////////////////////////////
--//            Secret Level
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
local BOW_LOT_NUM = 5628
local GUN_LOT_NUM = 5841

local MAX_HEALTH = 12
local NEW_HEALTH = 12


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	print("ON PLAYER LOADED")
	
	local player = msg.playerID
	
	-- Turn on PVP
	player:SetPVPStatus{ bOn = true }
	
	-- Add whatever items 
	additem(self, player, GUN_LOT_NUM)
	additem(self, player, BOW_LOT_NUM)
	
	-- Set their new health
	player:SetAttr{ string = "maxlife", value = MAX_HEALTH, ID = msg.playerID}
	player:SetAttr{ string = "life", value = NEW_HEALTH, ID = msg.playerID}
	
end


--------------------------------------------------------------
-- Add an item to the inventory if they don't already have it
--------------------------------------------------------------
function additem(self, player, lotnum)

	foundItem = false
	
	for i = 1, player:GetInventorySize{inventoryType = 1 }.size  do
		if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
			if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate == lotnum then
			  foundItem  = true
			end
		end
	end
	
	if not foundItem then
		local itemMsg = player:AddNewItemToInventory{ iObjTemplate = lotnum }
		-- player:EquipInventory{ itemtoequip = itemMsg.newObjID }  
	end
end
