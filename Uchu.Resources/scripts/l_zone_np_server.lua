--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
--require('L_ZONE_NP_SCENE_1')
--require('/zone/NP/L_ZONE_NP_SCENE_3')


--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["NUM_SCENES"] = 4



--------------------------------------------------------------
-- Clears all actors data
--------------------------------------------------------------
function ClearActorsData()

	ACTORS = {}					
	for sceneNum = 1, CONSTANTS["NUM_SCENES"] do
		ACTORS[sceneNum] = {}
	end

end


--------------------------------------------------------------
-- play actions on an object
--------------------------------------------------------------
function DoObjectAction(actor, type, action)

    -- spatial chat
    if (type == "chat") then
	    actor:DisplayChatBubble{wsText = action}
    
    -- animation
    elseif (type == "anim") then
	    local anim_time = actor:GetAnimationTime{  animationID = action }.time
	    if (tonumber(anim_time) > 0) then
			actor:PlayAnimation{animationID = action}
		end
 
	-- effect
    elseif (type == "effect") then		
		actor:PlayFXEffect{name = "N_" .. action, effectType = action }   

	elseif (type == "stopeffects") then
	   	actor:StopFXEffect{ name = "N_" .. action }				
    end

end


--------------------------------------------------------------
-- play actions on all actors in the scene
--------------------------------------------------------------
function DoSceneAction(scene, type, action)

    for actorID = 1, #ACTORS[scene] do

		-- get the actor out of the scene
	    local actor = GAMEOBJ:GetObjectByID(ACTORS[scene][actorID])
	    
	    -- perform the action
	    if (actor and actor:Exists()) then
			DoObjectAction(actor,type,action)
		end

    end

end


--------------------------------------------------------------
-- return if template is a valid actor
--------------------------------------------------------------
function IsValidActor(scene, templateID)

    local sceneStr = "SCENE_" .. scene .. "_VALID_ACTORS"
    
    -- list of actors does not exist
    if (CONSTANTS[sceneStr] == nil) then
        return false
    end
    
    -- look for a valid actor
	for actors = 1, #CONSTANTS[sceneStr] do
		if (templateID == CONSTANTS[sceneStr][actors]) then
			return true
		end
	end

	return false

end







--------------------------------------------------------------
-- Game Message Handlers
--------------------------------------------------------------

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 
	
    -- Scene Specific
    --Scene1Startup(self)
    --Scene2Startup(self)
    --Scene3Startup(self)
    --Scene4Startup(self)
    
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
       
    	-- Scene Specific
    --Scene1OnTimerDone(self, msg)
    --Scene2OnTimerDone(self, msg)
    --Scene3OnTimerDone(self, msg)
    --Scene4OnTimerDone(self, msg)
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	
end


--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

end

--------------------------------------------------------------
-- Generic notification message
--------------------------------------------------------------
function onNotifyObject(self, msg)

	-- Scene Specific
	--Scene1OnNotifyObject(self,msg)
	--Scene2OnNotifyObject(self,msg)
	--Scene3OnNotifyObject(self,msg)
    --Scene4OnNotifyObject(self, msg)
    
end


--------------------------------------------------------------
-- Sent from an object after loading into zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)

 	-- check for actors within each scene
	for scene = 1, CONSTANTS["NUM_SCENES"] do

	    if ( IsValidActor(scene, msg.templateID) == true ) then

            -- store the actor
            local nextActor = #ACTORS[scene] + 1
            ACTORS[scene][nextActor] = msg.objectID:GetID()
            
            --print("------------------- Adding Actor(" .. nextActor .. ") ID: " .. msg.objectID:GetID() .. " To scene " .. scene .. " template: " .. msg.templateID)
            break
            
	    end
        
	end	

end

