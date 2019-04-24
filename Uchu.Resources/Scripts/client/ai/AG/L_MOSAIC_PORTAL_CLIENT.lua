

--------------------------------------------------------------
-- startup
--------------------------------------------------------------
function onStartup( self ) 
	
	self:SetPickType{ ePickType = 14 }		-- PICK_LIST_INTERACTIVE from enum PICK_LIST_TYPE in lwoCommonVars.h
	
	self:SetProximityRadius{ radius = 200 }
end



--------------------------------------------------------------
-- when clicked on
--------------------------------------------------------------
function onClientUse( self, msg ) 
	
	-- get a list of all the objects near the mosaic and look through it for the maelstrom spike
	local objs = self:GetProximityObjects().objects

	local index = 1

	while index <= table.getn(objs)  do

		local target = objs[index]
		if (target) and (target:Exists()) then
		
			if ( target:GetLOT{}.objtemplate == 4019 ) then
				
				-- the mosaic portal can't zone the player itself because it doesn't exist on the server
				target:NotifyObject{ ObjIDSender = msg.user, name = "zonePlayer" }
			end
			
		end
		index = index + 1

	end
	
end


	


