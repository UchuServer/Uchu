-- When FireEvent is called on this object create a message box with the given text. 
-- Trigger format should be: MessageBox, your text here, *optional time in sec*
-- Created: 5/26/09 mrb...
local textVar = "%[SPIDER_CAVE_MESSAGE]"

require('o_mis')
-- default values for mBox
local mBox = {boxTarget = nil,
    isDisp = false,
    isTouch = false,
    isFirst = true,
    boxSelf = nil,
    boxText = '',
    boxTime = 1 }

function MakeBox()
    -- check to make sure we have a target
    if mBox.boxTarget == nil or mBox.isDisp or not mBox.boxSelf then return end
    
    mBox.isDisp = true
    --print('Creating Box')
    newTime = mBox.boxTime
    GAMEOBJ:GetTimer():AddTimerWithCancel( newTime, "BoxTimer", mBox.boxSelf )
    mBox.boxTarget:DisplayTooltip { bShow = true, strText = mBox.boxText, iTime = mBox.boxTime*1000 }
end

-- OnEnter in HF Trigger system 
function onCollisionPhantom(self, msg)
    -- Gets the target id that has collided
    if msg.objectID then 
        local dir = self:GetObjectDirectionVectors().forward
        
        mBox.boxTarget = msg.objectID        
        -- push-back effect 1378 -- PlayFXEffect
        msg.objectID:PlayFXEffect{name = "pushBack", effectID = 1378, effectType = "create"}--effectID = 1378, effectType = "push-back"}
        msg.objectID:PlayAnimation{ animationID = "knockback-recovery" }
        dir.y = dir.y + 15
        dir.x = dir.x * 100
        dir.z = dir.z * 100
        msg.objectID:Knockback { vector = dir }
    end        
    
    if not msg.objectID or mBox.isTouch  or mBox.isDisp then return end
    --print('starting EventTimer')
    mBox.boxSelf = self
    mBox.isTouch = true
    mBox.boxText = textVar
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "EventTimer", self )
end

-- OnExit in HF Trigger system
function onOffCollisionPhantom(self, msg )
    -- Says we have finished colliding tries to resetBox()
    if msg.objectID then 
        mBox.isTouch = false 
        resetBox()
        --print('Exiting')
    end
end

function onTimerDone(self, msg)    
    -- Says we are done with the displaying the message box, tries to resetBox()
    if msg.name == "BoxTimer" then
        mBox.isDisp = false
        resetBox()    
        --print('Box Timer Done')
    end
    -- checks to see if EventTimer has been called and if we are ready to do MakeBox(), need a valid mBox.boxTarget 
    if msg.name == "EventTimer" then
        if not mBox.boxTarget then
            --print('EventTimer not long enough.... running again')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "EventTimer", self )
            return
        end
        --print('EventTimer Done!!!')
        MakeBox()
    end
end

-- resets local data mBox
function resetBox() 
    -- checks to see if we are ready to reset mBox
    if mBox.isDisp or mBox.isTouch then return end
    -- default values
    mBox = {boxTarget = nil,
        isDisp = false,
        isTouch = false,
        isFirst = true,
        boxSelf = nil,
        boxText = '',
        boxTime = 1 }
end
