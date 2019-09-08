require('o_mis')

function onStartup(self) 

	
	self:ModifyHealth{amount = 10}
	self:SetRebuildState{iState = 2 } 

end
function onRebuildNotifyState(self, msg)
 
	if (msg.iState == 4 ) then
 	
 		local loc = self:GetPosition().pos
		RESMGR:LoadObject { objectTemplate =  6601  , x= loc.x , y=  loc.y , z=  loc.z  , owner = self } 
		
		local faction = self:GetFaction{}.faction
		
 	end
 
 
end

