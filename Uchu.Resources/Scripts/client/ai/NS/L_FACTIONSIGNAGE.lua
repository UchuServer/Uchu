--------------------------------------------------------------
-- Displays a fancy animation as you walk up to Faction Vendors
-- created by cassie
-- updated mrb... 1/18/10
--------------------------------------------------------------

-- OnEnter Proximity Radius
function onStartup(self,msg)
    -- set up the proximity radius
    self:SetProximityRadius { radius = 30 }
end

function onProximityUpdate(self, msg)
    local playerID = GAMEOBJ:GetLocalCharID()
    -- check to see if we are the correct player
    if playerID ~= msg.objId:GetID() then return end
    
    local animTime = 0
    local animName = "next"
    
    -- pick the correct animation based on enter or leave
    if (msg.status == "ENTER") then
        animName = "next"
        self:PlayAnimation{animationID = "next", bPlayImmediate = true}
    elseif (msg.status == "LEAVE") then 
        animName = "back"
    end    
    -- Get the animation time based on animName
    animTime = self:GetAnimationTime{animationID = animName}.time
    -- play the UI animation
    UI:SendMessage( "FactionSign_" .. animName, {}, self )
 
    -- cancel all timers and start one based on animName and animTime
    GAMEOBJ:GetTimer():CancelAllTimers( self )       
    GAMEOBJ:GetTimer():AddTimerWithCancel( animTime, animName, self )
    --print(animName .. ' ' .. animTime)
end

-- timers...
function onTimerDone(self, msg)
    -- play the correct idle animation
    if msg.name == "back" then           
		self:PlayAnimation{animationID = "Sign1", bPlayImmediate = true}
    elseif msg.name == "next" then             
		self:PlayAnimation{animationID = "Sign2", bPlayImmediate = true}
    end
end 