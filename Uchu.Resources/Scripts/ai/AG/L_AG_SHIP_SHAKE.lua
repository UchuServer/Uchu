local repTime = 2 -- base time for the first shake
local randTime = 10 -- number put into the math.random() to get the next shake
local shakeRad = 500.0 -- radius away from object clients should shake
local debrisObj = ''
--local randFX = {{'fx' = 'shipboom1', 'id' = 559},{'fx' = 'shipboom2', 'id' = 560},{'fx' = 'shipboom3', 'id' = 561}}
-- two different shake effect, only have one uncommented at a time
local fxName = "camshake-bridge" -- Lighter Shake with Angry animation
--local fxName = "camshake" -- Big Shake

function newTime()
    local time = math.random(randTime)
    if time < randTime/2 then time = time + randTime/2 end
    
    return repTime + time
end

function onStartup(self) 
    -- see if object has some varibles set in HF
    if self:GetVar("MaxRepTime") ~= nil then randTime = self:GetVar("MaxRepTime") end
    if self:GetVar("Radius") ~= nil then shakeRad = self:GetVar("Radius") end
    if self:GetVar("EffectName") ~= nil then fxName = self:GetVar("EffectName") end
    debrisObj = self:GetObjectsInGroup{ group = "DebrisFX"}.objects[1]    
    
    -- Initialize the pseudo random number generator and return 
    math.randomseed( os.time() )
    
    -- Do the first Shake Timer on start up
    GAMEOBJ:GetTimer():AddTimerWithCancel( repTime , "ShakeTimer", self )
end

function onTimerDone(self, msg)
    -- check to make sure there is a message and the timer name is correct
    local shipFXObj = self:GetObjectsInGroup{ group = "ShipFX"}.objects[1]
    local shipFX2Obj = self:GetObjectsInGroup{ group = "ShipFX2"}.objects[1]
    if msg.name == "ShakeTimer" then
        -- Start another shake timer with newTime()
        GAMEOBJ:GetTimer():AddTimerWithCancel( newTime(), "ShakeTimer", self )
        
        -- Shakes the players using the variables at the top of the script
        self:PlayEmbeddedEffectOnAllClientsNearObject{ radius = shakeRad, fromObjectID = self, effectName = fxName }
        
        -- Plays ship debris effect
        debrisObj:PlayFXEffect{ name  = "Debris", effectType = "DebrisFall"}       
        
        -- Plays ship FX       
        local randFX = math.random(3)

        shipFXObj:PlayFXEffect{ name  = "FX", effectType = 'shipboom' .. randFX, effectID = 559}
        --print('playFX **** ' .. randFX)
        local animTime = shipFXObj:GetAnimationTime{animationID =  'explosion' .. randFX}.time
        GAMEOBJ:GetTimer():AddTimerWithCancel( animTime, "ExplodeIdle", self )
        
        -- Plays ship FX2
        --shipFX2Obj:PlayFXEffect{ name  = "FX2", effectType = 'ship-shake', effectID = 578}
        shipFX2Obj:PlayAnimation{ animationID = 'explosion'}
    end
    if msg.name == "ExplodeIdle" then
        shipFXObj:PlayAnimation{ animationID = 'idle'}
        shipFX2Obj:PlayAnimation{ animationID = 'idle'}
        --print('reset FX idle ****')
    end
end
