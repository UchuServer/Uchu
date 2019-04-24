
function onStartup(self)
    local aggroRadius = 2
    self:SetProximityRadius { radius = aggroRadius }


end

function onProximityUpdate(self, msg)

		if msg.status == "ENTER" then
			
			if msg.objId:GetFaction().faction == 8  then
			  
			   
				 self:CastSkill{skillID = self:GetSkills().skills[1] } 
				 
			end
 		end

end 

