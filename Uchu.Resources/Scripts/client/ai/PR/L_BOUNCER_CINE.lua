require('o_mis')

--local bHasBeenUsed = false
--[[
local bCINEMA_ONCE = false



function onStartup(self)
    --print ("Bouncer Cine Script Started!")
    
    -- Set variable on self to remember if the Rancher has been found
    self:SetVar("RancherFound", false)                                                      

end
    

-- Something touches the phantom object
function onCollisionPhantom(self, msg)                                                      
    
    -- define the message sender from the collision as "player"    
    local player = msg.objectID                                                             
    
    -- Define myMissionState as the current state of mission 136, for the TargetID
    local PetFoodMissionStatus = player:GetMissionState{missionID = 111}.missionState       
    
    -- Cinematic should only happen once, and only during the Pet Food Mission
    if (bCINEMA_ONCE == false and PetFoodMissionStatus == 2) then                           
    
        -- Have we looked for the Rancher yet?
        if ( self:GetVar("RancherFound") == false) then                                     
	
			-- Call function to find the Rancher
            FindRancher(self)                                                               
			
            -- Make sure to never call function again 
            self:SetVar("RancherFound", true)                                               
	
	    end 
    
        --print ("Collision Detected")
        
        -- Disable player control and set a timer to return it
        --player:SetUserCtrlCompPause{bPaused = true}                                        
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "PauseTime", self )                      
        
        -- Play cinematic.
        player:PlayCinematic { pathName = "Bouncer_Cine" }                                  
        player:DisplayTooltip { bShow = true, strText = "CINEMATIC: Pet needs food. Pet Rancher gives food. Now try clicking your pet", iTime = 10000 }
        
        -- Call up the Pet Rancher from where it was stored on they cylinder (self)
        local PetRancher = getObjectByName(self, "PetRancher")                              

        -- Play animation on Pet Rancher (two methods)
        --PetRancher:PlayAnimation{animationID = "throw-food"}                              
        Emote.emote(PetRancher, PetRancher, "throw-food" )
        
        -- Update bool so that cinematic never plays again.
        bCINEMA_ONCE = true                                                                 

    end

end



function FindRancher(self)                        
  --print ("Inside FindRancher Function")

	local cylinder = self:GetObjectsInGroup{ group = self:GetVar("grp_name") }.objects
	
    for i = 1, table.maxn (cylinder) do
        
        --print ("looking for Rancher")
        --print (self:GetVar("grp_name"))
            
        --print(tostring(table.maxn (cylinder)))
        --print(tostring(cylinder[i]:GetLOT().objtemplate))
        
        
        
        if (cylinder[i]:GetLOT().objtemplate) == 3257  then                --3257 is Pet Rancher
        
        --print( type(cylinder[i]:GetLOT().objtemplate))
        --print( type(3257))
        		
        storeObjectByName(self, "PetRancher", cylinder[i])                  -- Stores the Pet Ranch ID under the name Pet Rancher
        storeObjectByName(cylinder[i], "cylinder", self)
        --print ("Rancher has been found")
            
        end

    end         
      	
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)
	
    if (msg.name == "PauseTime") then
        
        print ("Timer is done!")
	    
        -- Get the player again (local variables aren't shared by different functions) and return player control
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        --player:SetUserCtrlCompPause{bPaused = false}                                        

    end
	
end

 
]]--