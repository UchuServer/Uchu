require('o_mis')
-- Script is attached to 3630:pr-pet paw
-- When the player clicks on this "button" in the world, his pet will be sent to a path, and will perform any actions specified by that path.
-- To implement, create Custom Config Data on the Pet Paw object with the following: NAME=pathname, VALUE=0:<Name of Path>
-- The final Waypoint of your path should include Custom Config Data: NAME=reset

function onStartup(self)
     
end



onUse = function(self, msg )
        
      
        local user = msg.user	
        local pet = msg.user:GetPetID().objID
        
        if pet:GetVar("PathingActive") == nil or pet:GetVar("PetPathing") == false then
            pet:SetVar("PathingActive", true ) 
            -- Notify Pet to "Stay" --
            pet:SetTameness{fTameness = 5000 }
            pet:NotifyPet{ ObjIDSource = user,iPetNotificationType = 2} 
            pet:SetVar("WPEvent_NUM", 1)
            pet:SetVar("attached_path", self:GetVar("pathname") )
            pet:SetVar("attached_path_start", 1 )
            pet:FollowWaypoints()
	    end
    
end 

