--L_ACT_PLAYER_DEATH_TRIGGER.lua

-- instantly kills players when they touch anything with this script on it
require('o_mis')
--------------------------------------------------------------
-- onCollisionPhantom
--------------------------------------------------------------
function onCollisionPhantom (self,msg)
	local target = msg.objectID	
	
	-- If a player collided with me, then do our stuff
	if target:BelongsToFaction{factionID = 1}.bIsInFaction or target:BelongsToFaction{factionID = 100).bIsInFaction or BelongsToFaction{factionID = 101).bIsInFaction and target:IsDead().bDead == false then
	
	
		local FlagFound = removeflags(self, target)
		-- Return red flag
		if GAMEOBJ:GetZoneControlID():MiniGameGetTeam{ playerID = target}.teamID == 1 then
			if FlagFound then
				local redFlag =  GAMEOBJ:GetObjectByID(GAMEOBJ:GetZoneControlID():GetVar("Red_Flag_1"))
				local redpos =   redFlag:GetPosition().pos 
				GAMEOBJ:GetZoneControlID():SetVar("Con.Red_Flag_Home", true)
				RESMGR:LoadObject { objectTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlagOBJ") , x= redpos.x, y= redpos.y , z= redpos.z, owner = self }
				 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = target:GetName().name.."Killed him self, Red Flag Returned!!" }
			end
		end
		-- Return blue flag
		if GAMEOBJ:GetZoneControlID():MiniGameGetTeam{ playerID = target}.teamID == 2 then
			 local FlagFound = removeflags(self, target)
			 if FlagFound then
				local blueFlag =  GAMEOBJ:GetObjectByID(GAMEOBJ:GetZoneControlID():GetVar("Blue_Flag_1"))
				local bluepos =   blueFlag:GetPosition().pos 
				GAMEOBJ:GetZoneControlID():SetVar("Con.Blue_Flag_Home", true) 
				RESMGR:LoadObject { objectTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.BlueFlagOBJ") , x= bluepos.x, y= bluepos.y , z= bluepos.z}    
                GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "sendToAllclients_bubble" , paramStr = target:GetName().name.."Killed him self, Blue Flag Returned!!" }      
			end
        end
	
		target:Die{killerID = self}
		
	end	

end

function removeflags(self, player)

        for i =0, player:GetInventorySize{inventoryType = 1 }.size  do
            if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate == GAMEOBJ:GetZoneControlID():GetVar("Set.BlueFlag_LootOBJ") then
                   local item = player:GetInventoryItemInSlot{slot = 1 }.itemID
                   player:GetInventoryItemInSlot{slot = i }.itemID:UnEquipItem{bImmediate = true}
                   player:RemoveItemFromInventory{ iObjTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.BlueFlag_LootOBJ") }
                   return true
                end
            end
        end
        for i =0,  player:GetInventorySize{inventoryType = 1 }.size  do
            if player:GetInventoryItemInSlot{slot = i }.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i }.itemID:GetLOT{}.objtemplate == GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlag_LootOBJ") then
                   local item = player:GetInventoryItemInSlot{slot = 1 }.itemID
                   player:GetInventoryItemInSlot{slot = i }.itemID:UnEquipItem{bImmediate = true}
                   player:RemoveItemFromInventory{ iObjTemplate =  GAMEOBJ:GetZoneControlID():GetVar("Set.RedFlag_LootOBJ") }
                    return true
                end
            end
        end 

end