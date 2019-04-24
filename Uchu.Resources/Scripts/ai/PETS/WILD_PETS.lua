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

   local a = msg.objIDUser:GetInvItemCount{ itemID = 3191}.itemCount -- TUTORIAL Terrier Egg
   local b = msg.objIDUser:GetInvItemCount{ itemID = 3192}.itemCount -- TUTORIAL Cat Egg
   local c = msg.objIDUser:GetInvItemCount{ itemID = 3193}.itemCount -- TUTORIAL Reindeer Egg

   local BefriendMissionState = msg.objIDUser:GetMissionState{missionID = 109}.missionState
   local HatchMissionState    = msg.objIDUser:GetMissionState{missionID = 110}.missionState

    
   -- If befriending mission is Complete or ( Complete + anything else ), OR they already have an egg.

   -- If they finished befriending mission, 
   if (BefriendMissionState >= 4) then

       if ( a < 1 and b < 1 and c < 1 ) then   -- If they do not have a TUTORIAL egg

           if (HatchMissionState < 4) then     -- If they didn't hatch egg yet, and instead threw it away.
                msg.bCanUse = true

           end

       else    -- If they have a TUTORIAL egg

           msg.objIDUser:UseRequirementsResponse{iUseResponseType = 1 , objIDResponseSource = self, bCanUse = false }  --You can only tame one pet in the pet ranch
           msg.bCanUse =  false

       end
   
   -- If they haven't started the taming mission yet, don't let them tame a pet
   elseif (BefriendMissionState == 1) then                                        
       print("************* BefriendMissionState == 1 *************")
       msg.objIDUser:UseRequirementsResponse{iUseResponseType = 2 , objIDResponseSource = self ,bCanUse = false } --You cannot tame a pet until you've spoken to the ranch
       msg.bCanUse =  false 

   end 

   print("************* Done pet check! *************")
   return msg
   
end

