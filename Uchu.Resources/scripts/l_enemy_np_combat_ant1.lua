--------------------------------------------------------------
-- (SERVER SIDE) Passive Ants
--
--------------------------------------------------------------

require('o_mis')
require('L_NP_SERVER_CONSTANTS')

function onStartup(self) 
 
end


onTimerDone = function(self, msg)

     if msg.name == "Despawn" then 
            GAMEOBJ:GetTimer():CancelAllTimers( self )
            --self:Die{ killerID = self, killType = "SILENT" }
            GAMEOBJ:DeleteObject(self)
     end  
end

--------------------------------------------------------------
-- continue doign waypoints
-------------------------------------------------------------
function onArrived(self, msg)
	if (msg.wayPoint == CONSTANTS["ANT_FOOD_WAYPOINT"]) then
	
	    -- get random food LOT
        local foodLOT = CONSTANTS["ANT_FOOD_LOTS"][math.random(1,#CONSTANTS["ANT_FOOD_LOTS"])]
    
        -- add the item
		local itemMsg = self:AddNewItemToInventory{ iObjTemplate = foodLOT }  
		
		-- equip item 
		self:EquipInventory{ itemtoequip = itemMsg.newObjID }   
		
	end

	if (msg.isLastPoint == true) then
		Despawn(self)
	end    
end

--------------------------------------------------------------
-- Try to despawn the target
--------------------------------------------------------------
function Despawn(self)
		-- should we try to despawn?
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "Despawn", self )
end
