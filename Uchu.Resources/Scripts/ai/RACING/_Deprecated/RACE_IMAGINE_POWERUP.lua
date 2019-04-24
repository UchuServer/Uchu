local localskillID = 13
local ImaginationPickup = 20

function onStartup(self)
    self:SetVar("bIsDead", false)
end

function onCollisionPhantom(self, msg)

    if ( self:GetVar("bIsDead") == true ) then
        return
    end

    local target = msg.objectID
    local faction = target:GetFaction()
  	local isfaction = msg.senderID:GetFaction().faction
  	 	
   if isfaction == 0 then
	if ( (target:VehicleImaginationGetCurrent{}.iImagination) < 1000 ) then
		self:PlayFXEffect{effectType = "pickup"}
		self:CastSkill{skillID = localskillID, optionalTargetID = target}
		target:VehicleImaginationSetCurrent{bIgnoreMax = false, iImagination = target:VehicleImaginationGetCurrent{}.iImagination + ImaginationPickup }

		self:Die{ killerID = msg.playerID, killType = "SILENT" }
		self:SetVar("bIsDead", true)
		
 		
	end
   end
end