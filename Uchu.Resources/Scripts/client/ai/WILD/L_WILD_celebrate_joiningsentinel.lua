local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onStartup(self,msg)

    -- Start Facial Animation
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "FaceAnim1", self )
    -- Play a white flash when Nexus Symbol hits Minifig
    GAMEOBJ:GetTimer():AddTimerWithCancel( 6.15, "WhiteFlash", self )

end

function onTimerDone(self, msg)

    if (msg.name == "WhiteFlash") then
--Start fade
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.00, 0.2, true )--fades into white
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EndFlash", self )
-- Fades from white to normal
    elseif (msg.name == "EndFlash") then  
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.5, true )
-- Focused 1.0
    elseif (msg.name == "FaceAnim1") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.46, "FaceAnim2",self )
-- Shocked 2.46
    elseif (msg.name == "FaceAnim2") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.04, "FaceAnim3",self )
-- Angry 3.5
    elseif (msg.name == "FaceAnim3") then
        char:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.06, "FaceAnim4",self )
-- Focused 5.56
    elseif (msg.name == "FaceAnim4") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.77, "FaceAnim5",self )
-- Dizzy 6.33
    elseif (msg.name == "FaceAnim5") then
        char:PlayFaceDecalAnimation { animationID = "Dizzy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 3.33, "FaceAnim6",self )
-- Happy 9.66
    elseif (msg.name == "FaceAnim6") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.84, "FaceAnim7",self )
-- Shocked 10.5
    elseif (msg.name == "FaceAnim7") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.8, "FaceAnim8",self )
-- Focused 12.3
    elseif (msg.name == "FaceAnim8") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
    end

end