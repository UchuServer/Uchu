require('o_mis')
 --###########################################################
 --###              ON START UP                            ###
 --###########################################################
function onStartup(self)
  
end

 --###########################################################
 --#### ON CLICKED
 --###########################################################

onCheckUseRequirements = function (self, msg)

   local itemType = msg.objIDObjectForUse:GetItemType{}.iItemType
   if (itemType == 16) then   	-- If item is an Egg of some sort
   
      local itemLOT  = msg.objIDObjectForUse:GetLOT{}.objtemplate

      local HatchMissionState    = msg.objIDUser:GetMissionState{missionID = 110}.missionState

      -- CHECKING FOR INDIRECT USE, (Using an object or egg on the nest)
      print( "HatchMissionState ", HatchMissionState)
	
      if (HatchMissionState <= 1) then      -- If the hatch mission hasn't been accepted yet                                  

          -- Send response: "You cannot hatch egg until you've accepted that mission" 
          self:UseRequirementsResponse{iUseResponseType = 4, objIDResponseSource = self ,bCanUse = false }
          msg.bCanUse =  false 

      elseif ( itemLOT < 3191 and itemLOT > 3193 ) then	 -- If the item being used on nest is NOT a TUTORIAL egg
       
          -- Send response: "Nests in pet ranch accept specific tutorial eggs only" 
          self:UseRequirementsResponse{iUseResponseType = 3, objIDResponseSource = self ,bCanUse = false }
          msg.bCanUse =  false

      end

   else
      
      msg.bCanUse = true

   end

   return msg
    
end
