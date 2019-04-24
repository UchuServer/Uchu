-----------------------------------------------------------
--smashables for racing
-----------------------------------------------------------


--local localskillID = 13
local ImaginationPickup = 10


function onCollisionPhantom(self, msg)
   
    --print("I'm dead!")

   
    local target = msg.objectID
    local faction = target:GetFaction()
    local isfaction = msg.senderID:GetFaction().faction
  	
    local im = target:VehicleImaginationGetCurrent{}.iImagination
   
    if isfaction == 113 then
   
        local speed = target:GetCurrentSpeed{}.fSpeed
      
        --print("the speed was :" .. speed)
      
        self:Die{ killerID = target, directionRelative_Force = speed * 1.3 }
      
        if ( (target:VehicleImaginationGetCurrent{}.iImagination) < (target:VehicleImaginationGetMax{}.iMaxImagination) ) then
         
            target:PlayFXEffect{name = "bouncer", effectID = 194, effectType = "onbounce"}
            target:PlayFXEffect{name = "energy_orb", effectID = 1007, effectType = "cast"}
            --target:PlayFXEffect{name = "energy_orb", effectID = 1028, effectType = "on-anim"}
            --self:CastSkill{skillID = localskillID, optionalTargetID = target}
            target:VehicleImaginationSetCurrent{bIgnoreMax = false, iImagination =  im + ImaginationPickup }
      
        end
    end   
end