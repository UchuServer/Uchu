require('o_mis')
 
function onCollisionPhantom(self, msg)
		
	local target = msg.objectID
	local faction = target:GetFaction()

	if faction and faction.faction == 1 and  CheckSpanwPos(self) then
		local objs = GAMEOBJ:GetZoneControlID():GetVar("Con")
		if objs then
			for player in pairs(objs) do
				local playerID = GAMEOBJ:GetObjectByID(objs[player])
				if playerID:Exists() then

					 playerID:SetRespawnGroup{findClosest=true, respawnGroup= "spawn"..self:GetVar("tnum")}
				end
			end
		end
	end   
        
       
end


function CheckSpanwPos(self)

	if GAMEOBJ:GetZoneControlID():GetVar("currentSpawn") < tonumber(self:GetVar("tnum")) then
	
		GAMEOBJ:GetZoneControlID():SetVar("currentSpawn", tonumber(self:GetVar("tnum")) )
		
		return true 
	end

end