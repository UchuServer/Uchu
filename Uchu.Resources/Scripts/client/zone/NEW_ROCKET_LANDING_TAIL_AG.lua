
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('client/zone/L_ZONE_CUSTOM_ROCKET_INTRO')


--------------------------------------------------------------
-- Creating rocket for transition
--------------------------------------------------------------
function onStartup(self)

    local player = getObjectByName(self,"player")

    if (player:CharacterNeedsTransition{}.bNeedsTransition == false) then
        if GAMEOBJ:GetLocalCharID() == player:GetID() then
            StartZoneSummary(self)
        end
        return
    end



	local charPosition = player:GetPosition().pos
    local charRotation = player:GetRotation()
	local sceneID = LEVEL:GetSceneAtPos( charPosition )

	local spawnPos
	local spawnRot
	local spawnCinematic
	local spawnTransitionAnim

	spawnPos = {x = charPosition.x, y = charPosition.y, z = charPosition.z}
	spawnRot = charRotation
	spawnCinematic  = "LandingCine_" .. sceneID
	spawnTransitionAnim = "rocket-transition_default"

    if sceneID == 2 then
        spawnTransitionAnim = "rocket-transition_AG"
    end


--	if sceneID == 4 then
--		spawnPos = {x = 540.89, y = 404.03, z = 98.5}
--		spawnRot = {x = 0, y = -0.2927, z = 0, w = 0.9562}
--	else
--		spawnPos = {x = -407.78, y = 348.84, z = -157.66}
--		spawnRot = {x = 0, y = 0.804, z = 0, w = 0.594}
--	end



	-- other players always use the default transition animation, regardless
	if (player:GetID() ~= GAMEOBJ:GetLocalCharID()) then
		spawnTransitionAnim = "rocket-transition_default"
	end

	CUSTOMROCKET_BeginTransition(self, player, spawnPos, spawnRot, spawnCinematic, spawnTransitionAnim)

end

--------------------------------------------------------------
-- Rocket Rebuild Script
--------------------------------------------------------------

function onChildRenderComponentReady(self, msg)
	CUSTOMROCKET_onChildRenderComponentReady(self, msg)
end

function onChildBuildAssemblyComplete(self, msg)
	CUSTOMROCKET_onChildBuildAssemblyComplete(self, msg)
end

function onAnimationFinishedPreloading(self, msg)
	CUSTOMROCKET_onAnimationFinishedPreloading(self, msg)
end

function onTimerDone(self, msg)
	CUSTOMROCKET_onTimerDone(self, msg)

	-- (formerly was "FacialAnimationFocused", but it was renamed to make it generic and usable by more scripts)
    if (msg.name == "CustomLocalRocketTimer") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Focused", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.16, "FacialAnimationShocked",self )
    end

    if (msg.name == "FacialAnimationShocked") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.36, "FacialAnimationAngry",self )
    end

    if (msg.name == "FacialAnimationAngry") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Angry", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 3.36, "FacialAnimationBlink02",self )
    end

    if (msg.name == "FacialAnimationBlink02") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Blink", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 7.56, "FacialAnimationShocked02",self )
    end

    if (msg.name == "FacialAnimationShocked02") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Shocked", useAllDecals = true }
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.77, "FacialAnimationBlink03",self )
    end

    if (msg.name == "FacialAnimationBlink03") then  
        local localChar = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        localChar:PlayFaceDecalAnimation { animationID = "Blink", useAllDecals = true }
    end

end
