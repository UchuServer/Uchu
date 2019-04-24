--This script is unneeded now that the pet food container is a smashable. I will remove it soon.

--Click this item and you will have 5 pet food items added to your inventory
--This item is required to complete mission 111
--Petfood is 3227
--[[
local bCLICKONCE = false
local PETFOODQUANTITY = 5



function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
   --print ("Pet Food started up!")

end



----------------------------------------------------------------------
-- Player clicks on the item
----------------------------------------------------------------------
function onUse (self, msg)               --Check to make sure the script only runs once
    
    print ("inside onUse function")

        for i = 1, PETFOODQUANTITY do
            msg.user:AddItemToInventory { iObjTemplate  = 3227}       --Give the player 5 petfood items (3227)
            print ("inside for loop")
        end    
        
               
    if (bCLICKONCE == false) then       --Check to make sure the script only runs once
        bCLICKONCE = true               --Change the bool so the script won't run again
        print ("after click-once check")
        --msg.user:UpdateMissionTask{ target = self, value = 111, taskType= "complete" }   --"complete" actually sets it to "ready to complete" go figure.
        self:Die{ killType = "SILENT" }                                                 --Destroy the petfood
    
    end
    
end

]]--