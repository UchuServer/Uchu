require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"

function onCollisionPhantom(self, msg)
    local player = msg.senderID
    if (msg and removeBrick( player) ) then
       
      print("Brick Removed") 
    
    end


end

function removeBrick( player)

        for i =0, player:GetInventorySize{inventoryType = 4 }.size  do
            if player:GetInventoryItemInSlot{slot = i, inventoryType = 4  }.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4 }.itemID:GetLOT{}.objtemplate == GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Loot_Object") then
                    local item = player:GetInventoryItemInSlot{slot = 1 ,inventoryType = 4 }.itemID
                     player:GetInventoryItemInSlot{slot = i ,inventoryType = 4 }.itemID:UnEquipItem{bImmediate = true}
                     player:RemoveItemFromInventory{eInvType = 4 , iObjTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Loot_Object") }
                     
    
					 	
		    		 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "reSetAnimationSet" , paramObj = player }
		    		 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "HideDropButton" , rerouteID = player }
		    		 GAMEOBJ:GetZoneControlID():NotifyObject{name = "Captured_Object" , paramObj = target }
		    		 	
         
				     GAMEOBJ:GetZoneControlID():MiniGameAddPlayerScore{playerID = player, scoreType = 3, score = 1}  -- Capts
 
                     GAMEOBJ:GetZoneControlID():NotifyObject{name = "UpdateScore" }
                     
                     DoObjectAction(player, "stopeffects", "godlight")
                     
					 local str = player:GetName().name..GAMEOBJ:GetZoneControlID():GetVar("Set.Info_Text_7")
					 GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = str  }
					 player:SetSkillRunSpeed{Modifier = 500 }
					 
                     return true
                 
                end
            end
        end
end


