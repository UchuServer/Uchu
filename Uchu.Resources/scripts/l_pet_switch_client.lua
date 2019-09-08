
require('o_mis')
local bHasBeenUsed = false


function onCollisionPhantom(self, msg) -- Someting touches the Pet Switch
    	--print ("Message on collision")
	 
    	
	local target = msg.objectID -- get the ID of the thing that sent the collision message
	
	
	if target:BelongsToFaction{factionID = 99}.bIsInFaction then -- Check to see if the thing that sent the collision message is a pet
      
		
		
		local petswitch = getObjectByName(self, "pet_bouncer")	
    	self:PlayAnimation{animationID = "down"}
		
		petswitch:NotifyObject{name = "on"}
		
		
        if bHasBeenUsed == true then -- Do not display message more than once
            return        
        end
        
        local player = target:GetParentObj().objIDParent --get the ID of the player
        
            if ( (player) and (player:Exists()) ) then
                player:DisplayTooltip{ bShow = true, strText = "Click your pet - it will activate the switch!", iTime = 0 } --display hint message and leave it on until player turns it off
                bHasBeenUsed = true; --Don't ever display this tooltip again
            end
        
     	end
end 


function onOffCollisionPhantom (self,msg)
    local petswitch = getObjectByName(self, "pet_bouncer")	
	self:PlayAnimation{animationID = "up"}
	petswitch:NotifyObject{name = "off"}
	

end

