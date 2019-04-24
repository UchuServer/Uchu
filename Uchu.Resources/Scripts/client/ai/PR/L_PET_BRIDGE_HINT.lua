--require('o_mis')


function onStartup(self) -- When the Object with this script attacthed (self) loads or "starts up,"
    --print("started up TEST")

        self:SetProximityRadius{ radius = 8, name = "ALL"} 
        self:SetProximityRadius{ radius = 4, name = "LEAVE_MESSAGE"}
        
        self:SetVar("DISABLE", false) 
        self:SetVar("REBUILD_ICON_FOUND", false) 
        self:SetVar("BRIDGE_FOUND", false)
end


function onProximityUpdate(self, msg) -- returns objId 
--print (msg.status)
--print (msg.name)


    --Hint message will only be displayed when the player leaves the area, so this message won't conflict with the "you don't have enough bricks" message from the rebuild activator
    if msg.status == "LEAVE" and msg.name == "LEAVE_MESSAGE" and msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction then 
    --print ("left region")
        
        -- If the hint message is not currently disabled, proceed
        if (self:GetVar("DISABLE") == false) then
            
            FindBridge(self)      
            
            -- Look to see what objects are nearby
            local foundObj = self:GetProximityObjects { name = "ALL"}.objects
            for i = 1, table.maxn (foundObj) do  
                
                --Check to see if the rebuild activator (1692) is present
                if (foundObj[i]:GetLOT().objtemplate == 1692) then
                    --print ("rebuild activator found")
                    self:SetVar("REBUILD_ICON_FOUND", true)
                    break
                end
                
            end 
            
            --If the rebuild Icon is present AND the bridge is present
            if ((self:GetVar("REBUILD_ICON_FOUND")) and (self:GetVar("BRIDGE_FOUND") == true )) then
                
                self:SetVar("DISABLE", true)
                local player = msg.objId -- get the ID of the thing that sent the collision message
                player:DisplayTooltip{ bShow = true, strText = "That broken bridge has the bricks I need!", iTime = 5000 } --display hint message and leave it on until player turns it off
                GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "temp_disable", self )   
            end
            
        end     
        
    end
   
end


onTimerDone = function(self, msg)

    if msg.name == "temp_disable" then 
         self:SetVar("DISABLE", false) 
         self:SetVar("REBUILD_ICON_FOUND", false) 
         self:SetVar("BRIDGE_FOUND", false) 
    end
  
end 



---------------------------------------------------------------------------------------
-- Check to see if the broken bridge is there or not
---------------------------------------------------------------------------------------
function FindBridge(self)                        
  --print ("Inside FindBridge Function")

	local cylinder = self:GetObjectsInGroup{ group = self:GetVar("grp_name") }.objects
	
    for i = 1, table.maxn (cylinder) do
        
        --print ("looking for bridge")
        
        if ((cylinder[i]:GetLOT().objtemplate) == 3881)  then                --3881 is bridge smashable
        --print ("Bridge has been found")
        self:SetVar("BRIDGE_FOUND", true) 
            
        end
    end         

end
