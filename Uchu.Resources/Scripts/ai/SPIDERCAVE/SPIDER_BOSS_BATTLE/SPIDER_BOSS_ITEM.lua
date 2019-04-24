

require('o_mis')


function onStartup(self)

	print "Item Script Started"
	
end

function onCollisionPhantom(self, msg)
		
	local target = msg.objectID
	local faction = target:GetFaction()
	
	if faction and faction.faction == 1 then
		
		
		 print "hit Chest"
		 local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuildP" }.objects
					  
				    obj[1]:SetModelToBuild{templateID=6783}	
					    
         obj[1]:SpawnModelBricks{amount=0.1, pos=  self:GetPosition().pos }
         
         
         
	end   
        
       
end