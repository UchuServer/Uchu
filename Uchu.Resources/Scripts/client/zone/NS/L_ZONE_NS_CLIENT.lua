
--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
-- these scripts contain specific scene functionality for the client zone object
--require('client/zone/NS/L_ZONE_NS_KIPPER_DUEL_CLIENT')
require('client/zone/L_ZONE_CUSTOM_ROCKET_INTRO')

-- Actor storage by scene for zone
ACTORS = {}     

--------------------------------------------------------------
-- Clears all actors data
--------------------------------------------------------------
--function ClearActorsData()
--    ACTORS = {}
--    ACTORS["KipperDuel"] = {}
--end

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
--function onStartup(self) 
--    ClearActorsData()
--    -- Scene Specific
--    onStartupKipperDuel(self)
--end

--------------------------------------------------------------
-- return if template is a valid actor
--------------------------------------------------------------
--function IsValidActor( sceneName, templateID )
--    local sceneStr = "sceneName" .. "_VALID_ACTORS"
--    -- list of actors does not exist
--    if ( CONSTANTS[ sceneStr ] == nil ) then
--        return false
--    end    

--    -- look for a valid actor
--    for actorLOT = 1, #CONSTANTS[ sceneStr ] do
--        if ( templateID == CONSTANTS[sceneStr][actorLOT] ) then
--            return true
--        end
--    end
--    return false
--end
 
--------------------------------------------------------------
-- Generic message from a specific object
--------------------------------------------------------------
--function onFireEvent( self, msg )
--    -- object is telling us it is ready and to set its scene state
--    -- based on the completion of a scene
--    if ( msg.args == "ActorReadyKipperDuel" ) then
--        --print( "client-side zone script - onFireEvent - ActorReadyKipperDuel" )
--        ActorReadyKipperDuel( self, msg.senderID )
--    elseif ( msg.args == "ModelReadyKipperDuel" ) then
--        ModelReadyKipperDuel( self, msg.senderID )
--    end
--end 

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
--function onChildLoaded(self, msg)
--    -- Scene Specific
--    onChildLoadedKipperDuel( self, msg )           
--end

--------------------------------------------------------------
-- Creating rocket for transition
--------------------------------------------------------------
function onCharacterUnserialized(self, msg)

    if (msg.charID:CharacterNeedsTransition{}.bNeedsTransition == false) then return end

	--print("ROCKET TRANSITION BEGIN: PLAYER ID " .. tostring(msg.charID:GetID()) .. " !!!!!")

	local charPosition = msg.charID:GetPosition().pos
	local sceneID = LEVEL:GetSceneAtPos( charPosition )

	local spawnPos
	local spawnRot
	local spawnCinematic
	local spawnTransitionAnim

	if sceneID == 4 then
		spawnPos = {x = 540.89, y = 406.03, z = 98.5}
		spawnRot = {x = 0, y = -0.2927, z = 0, w = 0.9562}
		spawnCinematic = ""
		spawnTransitionAnim = "rocket-transition_default"
	else
		spawnPos = {x = -136.11, y = 285.39, z = 209.43}
		spawnRot = {x = 0, y = 145, z = 0, w = 0}
		spawnCinematic = "Landing_From_AG"
		spawnTransitionAnim = "rocket-transition_AG"
	end

	-- other players always use the default transition animation, regardless
	if (msg.charID:GetID() ~= GAMEOBJ:GetLocalCharID()) then
		spawnTransitionAnim = "rocket-transition_default"
	end

	CUSTOMROCKET_BeginTransition(self, msg.charID, spawnPos, spawnRot, spawnCinematic, spawnTransitionAnim)

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
    -- Scene Specific Kipper Dual
    onTimerDoneKipperDuel( self, msg )
    
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
