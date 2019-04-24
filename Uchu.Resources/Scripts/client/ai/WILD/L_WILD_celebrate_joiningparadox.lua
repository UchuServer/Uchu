local char = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

function onStartup(self,msg)

    -- Turn off UI
    --UI:SendMessage( "pushGameState", {{"state", "Celebration" }} )

    -- Start Facial Animation
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.267, "FaceAnim1", self )
    -- Play a white flash when Nexus Symbol hits Minifig
    GAMEOBJ:GetTimer():AddTimerWithCancel( 3.7, "WhiteFlash", self )
    -- Turn on UI
    GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetAnimationTime{ animationID = "idle" }.time, "Reset", self )

end

function onTimerDone(self, msg)

    if (msg.name == "WhiteFlash") then
--Start fade
        LEVEL:FadeEffect(.5, 0, 1.0, 0.0, .5, 0, 1.0, 1.00, 0.2, true )--fades into white
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EndFlash", self )
-- Fades from white to normal
    elseif (msg.name == "EndFlash") then  
        LEVEL:FadeEffect( .5, 0, 1.0, 1.0, .5, 0, 1.0, 0.0, 1.5, true )
-- Focused 0.1
    elseif (msg.name == "FaceAnim1") then
        char:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.2, "FaceAnim2",self )
-- Angry 2.3
    elseif (msg.name == "FaceAnim2") then
        char:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.3, "FaceAnim3",self )
-- Shocked 3.6
    elseif (msg.name == "FaceAnim3") then
        char:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "FaceAnim4",self )
-- Frustrated 5.6
    elseif (msg.name == "FaceAnim4") then
        char:PlayFaceDecalAnimation { animationID = "Frustrated", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.4, "FaceAnim5",self )
-- Angry 7.0
    elseif (msg.name == "FaceAnim5") then
        char:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
-- Reenable UI
    elseif ( msg.name == "Reset" ) then
       -- UI:SendMessage( "popGameState", {{"state", "Celebration" }} )
    end

end