-- When the player walks through a phantom physics box with this script attached, the pet will begen to follow a path. 
-- Based on PET_INTERACT_PAW.lua, sends the pet on a path when the player clicks on an object.


-- TO SET THIS UP: In the phantom physics box's config data, type in "pathname" under NAME and "0:<name of your path> under VALUE.

function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
   
         
end



function onCollisionPhantom(self, msg)
      --print ("collided with invisible box")
      
                local target = msg.objectID	
                local pet = target:GetPetID().objID

                local faction = target:GetFaction()

                -- If a player collided with me, then do our stuff
                    if faction and faction.faction == 1 then

                        pet:SetVar("AiDisabled", false)
                        pet:AddPetState{iStateType = 10 } 
                        -- pet:SetPetMovementState{iStateType = 7}

                        --pet:SetPetState{iStateType = 9 }

                        pet:SetVar("PathingActive", true ) 
                        -- Notify Pet to "Stay" --
                        -- pet:SetTameness{fTameness = 5000 }

                        pet:SetVar("WPEvent_NUM", 1)
                        pet:SetVar("attached_path", "Bridge_Path" )
                        pet:SetVar("attached_path_start", 0 ) 
                        pet:FollowWaypoints()
      
                end
      
       	
end 

