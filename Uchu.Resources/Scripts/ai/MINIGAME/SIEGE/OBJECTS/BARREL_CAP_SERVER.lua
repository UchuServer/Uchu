CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"
require('o_mis')
function onStartup(self)

  --  GAMEOBJ:GetZoneControlID():NotifyObject{ name="Barrel_Loaded", ObjIDSender = self   }
   
    self:SetVar("PickUpReady", false)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "ReadyToBePickedUp", self )	
   -- print( tostring(self:GetVar("taken_name")) )
   
   
	if self:GetVar("AtHomePoint") == false then
		GAMEOBJ:GetTimer():AddTimerWithCancel( GAMEOBJ:GetZoneControlID():GetVar("Set.Barrel_Reset")  , "ReturnToHomePoint" , self )
	end
end

function checkBrick(player)

        for i =0, player:GetInventorySize{inventoryType = 4 }.size  do
            if player:GetInventoryItemInSlot{slot = i ,inventoryType = 4}.itemID:Exists() then
                if player:GetInventoryItemInSlot{slot = i, inventoryType = 4 }.itemID:GetLOT{}.objtemplate == GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Loot_Object") then
                  
                       return true
                 
                end
            end
        end
end

function onCollisionPhantom(self, msg)
   local lootID = GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Loot_Object")
   local target =  msg.objectID
   local faction = target:GetFaction()
   
   local team = GAMEOBJ:GetZoneControlID():MiniGameGetTeam{playerID = target }.teamID 
    if  self:GetVar("PickUpReady") and team == 2 then
    
           local brickcheck = checkBrick(target)
           if not brickcheck then
           		GAMEOBJ:GetTimer():CancelAllTimers( self )
                target:HideEquipedWeapon()
                
                self:Die{ killerID = msg.playerID, killType = "SILENT" }
                self:SetVar("bIsDead", true)
              
               
               itemMsg =  target:AddNewItemToInventory{ invType = 4, iObjTemplate = lootID }
               target:EquipInventory{ itemtoequip = itemMsg.newObjID } 
               
               target:SetSkillRunSpeed{Modifier = GAMEOBJ:GetZoneControlID():GetVar("Set.Barrel_Speed") }
               
               
               DoObjectAction(target, "effect", "godlight")
               
               for i = 1, 3 do
               		if self:GetVar("taken_name") == "Blue_Point_"..i then
               		
               			itemMsg.newObjID:SetVar("taken_name","Blue_Point_"..i) 
               			
               		end
               
               end
              
               
               
               itemMsg.newObjID:NotifyObject{name= "EQ"}
               
               GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SetAnimationSet" , paramStr = "carry" , paramObj = target }
               GAMEOBJ:GetZoneControlID():NotifyObject{ name = "removeBarrel" , ObjIDSender = self}
            
            	-- Show UI Button
            	GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "ShowDropButton" , rerouteID = target }
            	
            	local str = target:GetName().name..GAMEOBJ:GetZoneControlID():GetVar("Set.Info_Text_4")
                GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = str  }
                
                
            end
    end
    
    if  self:GetVar("PickUpReady") and team == 1  and not self:GetVar("AtHomePoint") then
    	GAMEOBJ:GetTimer():CancelAllTimers( self )
        self:Die{ killerID = msg.playerID, killType = "SILENT" }
        local obj = getObjectByName( GAMEOBJ:GetZoneControlID(), self:GetVar("taken_name") )
		local bluepos =   obj:GetPosition().pos 
		local config = { {"AtHomePoint", true }, {"taken_name",self:GetVar("taken_name")}   }
		RESMGR:LoadObject { owner = GAMEOBJ:GetZoneControlID(), objectTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Object_LOT") , x= bluepos.x, y= bluepos.y , z= bluepos.z, configData = config  } 
		
		GAMEOBJ:GetZoneControlID():MiniGameAddPlayerScore{playerID = target, scoreType = 4, score = 1 } -- retrun
		GAMEOBJ:GetZoneControlID():NotifyObject{name = "UpdateScore" }
		
		local str = target:GetName().name..GAMEOBJ:GetZoneControlID():GetVar("Set.Info_Text_6")
		GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SendTxt_TeamMsgbox" , paramStr = str  }		
		
		
        
    end
    
end 
--------------------------------------------------------------
-- object notification
--------------------------------------------------------------
function onNotifyObject( self, msg )
	
	if ( msg.name == "unequip" ) then
	
		self:UnEquipItem{ bImmediate = true }

	
	end
	if ( msg.name == "killself" ) then
		 self:Die{ killerID = msg.playerID, killType = "SILENT" }
	end
	

	
		     GAMEOBJ:GetTimer():AddTimerWithCancel( 3 , "ReturnToHomePoint" , self )	
		     

	
	
end

function onTimerDone(self,msg)

    if msg.name == "ReadyToBePickedUp" then
      self:SetVar("PickUpReady", true)
    end

    if msg.name == "ReturnToHomePoint" then
    		 self:Die{ killerID = msg.playerID, killType = "SILENT" }
     		local obj = getObjectByName( GAMEOBJ:GetZoneControlID(), self:GetVar("taken_name") )
			local bluepos =   obj:GetPosition().pos 
			local config = { {"AtHomePoint", true }, {"taken_name",self:GetVar("taken_name")}   }
			RESMGR:LoadObject { owner = GAMEOBJ:GetZoneControlID(), objectTemplate = GAMEOBJ:GetZoneControlID():GetVar("Set.QB_Object_LOT") , x= bluepos.x, y= bluepos.y , z= bluepos.z, configData = config  } 
    
    end

end