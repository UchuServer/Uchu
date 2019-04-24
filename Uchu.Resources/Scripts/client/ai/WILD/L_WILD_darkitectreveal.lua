local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onRenderComponentReady(self,msg)
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.56, "BridgeEnter", self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 11.7, "BeginFullScreenFlash", self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 11.98, "FullScreenFlash", self )         
    GAMEOBJ:GetTimer():AddTimerWithCancel( 17.83, "DarkitectSmash", self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 19.83, "HealthDrain", self )

end

function onTimerDone(self, msg)
    if (msg.name == "Flash") then
        LEVEL:ModifyEnvironmentSettings{                         
                                fogColor = {r = 0, g = 0, b = 0},  
                                blendTime = .5,                                
                                minDrawDistances = {fogNear = 100, fogFar = 400, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000},
                                maxDrawDistances = {fogNear = 100, fogFar = 400, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000}
                                }
    elseif (msg.name == "EndFlash") then 
            LEVEL:ModifyEnvironmentSettings{                         
                                fogColor = {r = 0, g = 0, b = 0},  
                                blendTime = .8,
                                minDrawDistances = {fogNear = 2000, fogFar = 2000, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000},
                                maxDrawDistances = {fogNear = 2000, fogFar = 2000, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000}
                                }
    else  
        UI:SendMessage( "TimerUpdate", {{"frameLabel", msg.name }} )
    end
end

