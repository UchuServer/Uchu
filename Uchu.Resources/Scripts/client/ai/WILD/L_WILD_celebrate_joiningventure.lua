local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onStartup(self,msg)

    -- Start Facial Animation
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "FaceAnim1", self )
    -- Play a white flash when Nexus Symbol hits Minifig
    GAMEOBJ:GetTimer():AddTimerWithCancel( 2.7, "WhiteFlash", self )

end

function onTimerDone(self, msg)

    if (msg.name == "WhiteFlash") then
--Start fade
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.00, 0.2, true )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EndFlash", self )
-- Fades from white to normal
    elseif (msg.name == "EndFlash") then  
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.5, true )
-- Shocked 1.5
    elseif (msg.name == "FaceAnim1") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.7, "FaceAnim2",self )
-- Happy 2.2
    elseif (msg.name == "FaceAnim2") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.6, "FaceAnim3",self )
-- Dizzy 2.8
    elseif (msg.name == "FaceAnim3") then
        char:PlayFaceDecalAnimation { animationID = "Dizzy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.4, "FaceAnim4",self )
-- Happy 4.2
    elseif (msg.name == "FaceAnim4") then
        char:PlayFaceDecalAnimation { animationID = "Blink", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.3, "FaceAnim5",self )
-- Shocked 6.5
    elseif (msg.name == "FaceAnim5") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.8, "FaceAnim6",self )
-- Focused 7.3
    elseif (msg.name == "FaceAnim6") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 4.7, "FaceAnim7",self )
-- Angry 12.0
    elseif (msg.name == "FaceAnim7") then
        char:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.8, "FaceAnim8",self )
-- Focused 12.8
    elseif (msg.name == "FaceAnim8") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.4, "FaceAnim9",self )
-- Shocked 13.2
    elseif (msg.name == "FaceAnim9") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.3, "FaceAnim10",self )
-- Happy 14.5
    elseif (msg.name == "FaceAnim10") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
    end

end