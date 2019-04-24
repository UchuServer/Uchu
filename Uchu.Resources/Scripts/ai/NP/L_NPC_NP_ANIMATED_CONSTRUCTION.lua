--------------------------------------------------------------
-- (SERVER SIDE) Script for animated dwarf/troll in scene 1
--------------------------------------------------------------


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)

	self:FollowWaypoints()
	
end

--------------------------------------------------------------
-- Equips item and plays carry animation
--------------------------------------------------------------
function CarryItem(self)

	-- equip an item
    local meItem = self:GetInventoryItemInSlot().itemID
    self:EquipInventory{ itemtoequip = meItem}
	
	-- play carry animation
    self:PlayAnimation{animationID = "carry"}

end

--------------------------------------------------------------
-- UnEquips item and plays no carry animation
--------------------------------------------------------------
function DropItem(self)

    -- unequip item
    local meItem = self:GetInventoryItemInSlot().itemID
    self:UnEquipInventory{ itemtounequip = meItem}
    self:PlayAnimation{animationID = "nocarry"}

end


--------------------------------------------------------------
-- When object arrives at a waypoint
--------------------------------------------------------------
function onArrived(self, msg)

    -- are we here because of a path?
    if (msg.pathType == "Waypoint") then
        
        if (msg.isLastPoint == true) then
            CarryItem(self)
        elseif (msg.wayPoint == 0) then
            DropItem(self)
        end

    end
    self:ContinueWaypoints()

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- keep moving
    if (msg.name == "Continue") then
	    
	    DropItem(self)
        self:FollowWaypoints()

    end
	
end