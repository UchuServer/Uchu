--------------------------------------------------------------
-- (SERVER SIDE) Script for dirt drop volume
--
-- Set the following config data:
-- markedAsPhantom  7:1
-- renderDisabled   7:1
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------
local defaultProx = 8.0
local cooldown = 5.0
local stopDuration = 4.0
local brickDropDuration = 1.0


--------------------------------------------------------------
-- event fired
--------------------------------------------------------------
function onFireEvent(self, msg)

    if (msg.args == "dirtDrop") then
        -- if the cooldown isn't up, quit out

        if (self:GetVar("Usable") == false) then
            return
        else
            -- otherwise set us as not usable (because we are using now)
            self:SetVar("Usable", false)
            GAMEOBJ:GetTimer():AddTimerWithCancel( cooldown, "Cooldown",self )
        end
    
        -- play embedded effects 
        msg.senderID:PlayFXEffect{effectType = "press"}
        
        -- if its smashable, smash it
        local smashMsg = self:IsObjectSmashable()
        if (smashMsg) and (smashMsg.smashable == true) then
            self:Die()
        end
        
        -- start a timer to stop targets under the brick drop area when the bricks fall
        GAMEOBJ:GetTimer():AddTimerWithCancel( brickDropDuration, "StopTargets",self )
        
    end

end


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)

    self:SetVar("Usable", true)
    self:SetProximityRadius{radius = defaultProx}
    
end

    
--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- set our state as usable
    if (msg.name == "Cooldown") then
	    self:SetVar("Usable", true)
    
    elseif (msg.name == "StopTargets") then
    
        -- execute stuff on objects in proximity
        local objs = self:GetProximityObjects().objects
        local index = 1

        while index <= table.getn(objs)  do

            local target = objs[index]
            if (target) and (target:Exists()) then
            
        		GAMEOBJ:GetTimer():CancelAllTimers( target )

                -- stop pathing
                target:StopPathing()

                -- unequip item
                local meItem = target:GetInventoryItemInSlot().itemID
                target:UnEquipInventory{ itemtounequip = meItem}
        		
        		-- play effect/anim
                target:PlayAnimation{animationID = "dirtDrop"}
                
                -- start timer to resume pathing (based on time of animation?)
                GAMEOBJ:GetTimer():AddTimerWithCancel( stopDuration, "Continue",target )
                
            end
            index = index + 1

        end
        
    end
	
end 