local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onStartup(self,msg)

    -- Start Facial Animation
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.3, "FaceAnim1", self )
    -- Play a white flash when Nexus Symbol hits Minifig
    GAMEOBJ:GetTimer():AddTimerWithCancel( 4.12, "WhiteFlash", self )--start white flash at this time

end

function onTimerDone(self, msg)

    if (msg.name == "WhiteFlash") then
--Start fade
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.00, 0.2, true )--fades into white
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EndFlash", self )
-- Fades from white to normal
    elseif (msg.name == "EndFlash") then  
        LEVEL:FadeEffect( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.5, true )
-- Happy 1.3
    elseif (msg.name == "FaceAnim1") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.1, "FaceAnim2",self )
-- Shocked 2.4
    elseif (msg.name == "FaceAnim2") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.1, "FaceAnim3",self )
-- Angry 3.5
    elseif (msg.name == "FaceAnim3") then
        char:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.9, "FaceAnim4",self )
-- Dizzy 4.4
    elseif (msg.name == "FaceAnim4") then
        char:PlayFaceDecalAnimation { animationID = "Dizzy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.3, "FaceAnim5",self )
-- Focused 6.7
    elseif (msg.name == "FaceAnim5") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.4, "FaceAnim6",self )
-- Happy 8.1
    elseif (msg.name == "FaceAnim6") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 6.7, "FaceAnim7",self )
-- Focused 14.8
    elseif (msg.name == "FaceAnim7") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5.7, "FaceAnim8",self )
-- Happy 20.5
    elseif (msg.name == "FaceAnim8") then
        char:PlayFaceDecalAnimation { animationID = "Happy", useAllDecals = true }
    end

end