function onClientUse(self, msg)    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    local obj = self:GetObjectsInGroup{ group = "Jet_FX", ignoreSpawners = true }.objects[1]
        
    if self:GetLOT().objtemplate == 6859 and not self:GetVar('isInUse') then
        --print('pass client')
        obj:PlayAnimation{ animationID = "jetFX" }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "PlayCine", self )
    end
end 

----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHide
----------------------------------------------
function toggleActivatorIcon(self, bHide)
    if not bHide then
        bHide = false
    end
    
    self:SetIconAboveHead{iconMode = 1, iconType = 69, bIconOff = bHide}
end

function onNotifyClientObject(self, msg) 
    if msg.name == "toggleInUse" then
        if msg.param1 == -1 then
            --print('rdy')
            self:SetVar('isInUse', false)
            toggleActivatorIcon(self)
        else
            --print('inUse')
            toggleActivatorIcon(self, true)
            self:SetVar('isInUse', true)
        end
    end
end

function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
       msg.fCurrentPickTypePriority = myPriority  
       msg.ePickType = 14    -- Interactive pick type     
    end  
  
    return msg      
end 

function onTimerDone (self,msg)
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    if (msg.name == "PlayCine") then              
        player:PlayCinematic{ pathName = "Jet_Cam_01", leadIn = 2 }    
    end
end 