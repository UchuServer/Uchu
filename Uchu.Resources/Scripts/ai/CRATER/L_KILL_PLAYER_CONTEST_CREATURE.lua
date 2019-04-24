--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
local aggroRadius = 50
local localskillID = 69

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

	self:SetProximityRadius { radius = aggroRadius, FOVradius = 45}
	
end


--------------------------------------------------------------
-- On proximity
--------------------------------------------------------------

function onProximityUpdate(self, msg)
	if msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 or msg.objId:GetFaction().faction == 101 then
		 --print "just entered the if statement"
	 	 	self:KillObj{targetID = msg.objId}
		--print "i just cast a skill"
	end
end






