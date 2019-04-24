-- Designed for use with the wall trigger object, will move 
-- player which ever direction you rotate the z axis(the up face in HF)
-- Created: 4/29/09 mrb...

local pID = nil
local forceMod = 2


function onStartup(self)
    self:SetVar('isOn', true)
    self:SetVar('isColliding', false)
end

-- OnEnter in HF Trigger system
function onCollisionPhantom(self, msg)
    -- Gets the target id that has collided
    if msg.objectID and self:GetVar('isOn') then 
        self:SetVar('isColliding', true)
        pID = msg.objectID
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PushTimer", self)  
    end        
end

-- OnExit in HF Trigger system
function onOffCollisionPhantom(self, msg )
    -- Says we have finished colliding tries to resetBox()
    if msg.objectID then 
        self:SetVar('isColliding', false)
        --print('Exiting')
        pID = nil
        forceMod = 2
    end
end

function onTimerDone(self, msg)  
    if not pID then return end
    
    -- keep pushing while isColliding == true
    if msg.name == "PushTimer" and self:GetVar('isColliding') and self:GetVar('isOn') then
        local oDir = {z = self:GetVar('DirX'),y = self:GetVar('DirY'),x = self:GetVar('DirZ')} 
               
        -- pushes the player back based on the forceMod * the z rotation of the object 
        pID:Knockback{vector={x=oDir.x,y=oDir.y ,z=forceMod * oDir.z}} 
        
        -- tick the forceMod to get the player out 
        forceMod = forceMod + 0.5             

        -- start the next timer
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "PushTimer", self)  
    end
end

function onFireEvent(self, msg)
    if msg.args == 'off' then
        self:SetVar('isOn', false)
    end
    if msg.args == 'on' then    
        self:SetVar('isOn', true)
    end
end
