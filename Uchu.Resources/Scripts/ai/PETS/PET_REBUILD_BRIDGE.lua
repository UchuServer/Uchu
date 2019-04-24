require('o_mis')


function onStartup(self)
   
         
end


function onCollision(self, msg)
   

		local target = msg.objectID	
		local pet = target:GetPetID().objID

		if target:BelongsToFaction{factionID = 1}.bIsInFaction then

				pet:SetVar("AiDisabled", false)
				pet:AddPetState{iStateType = 10 } 
				pet:SetVar("PathingActive", true ) 
				pet:SetVar("WPEvent_NUM", 1)
				pet:SetVar("attached_path", "Bridge_Path" )
				pet:SetVar("attached_path_start", 0 ) 
				pet:FollowWaypoints()

		end
      
       	
end 




