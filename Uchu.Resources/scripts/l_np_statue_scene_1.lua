--------------------------------------------------------------
-- (CLIENT SIDE) Changing statues in scene 1
--
-- On proximity of the local character, will change statue.
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')


--------------------------------------------------------------
-- Startup of object
--------------------------------------------------------------
function onStartup(self)

    -- current Model's LOT
    self:SetVar("CurrentModel",CONSTANTS["LOT_NULL"])
    
    -- flag for tracking if statue is ready to change
    SetIsReady(self,true)
    
	-- get a model LOT
	local newLOT = SelectNewModel(self, CONSTANTS["SCENE_1_STATUE_LOTS"])

	-- spawn a model
	SpawnModel(self, newLOT)
		
end


--------------------------------------------------------------
-- Called when rendering is complete for this object
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

	self:SetProximityRadius { radius = CONSTANTS["SCENE_1_STATUE_PROX_RADIUS"] }

end


--------------------------------------------------------------
-- Called when an entity gets within proximity of the object
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	-- if the local player is close enough to us
	if (msg.status == "ENTER") and (IsReady(self)) and (msg.objId:GetID() == GAMEOBJ:GetLocalCharID()) then

        -- spawn a random model
        local newLOT = SelectNewModel(self, CONSTANTS["SCENE_1_STATUE_LOTS"])
        SpawnModel(self, newLOT)

    end
    
end


--------------------------------------------------------------
-- Picks a random model and returns the LOT. Will not pick
-- the currently displayed model's LOT
--------------------------------------------------------------
function SelectNewModel(self, lotTable)

    -- cases of 0 or 1 statue LOT
    if (#lotTable < 1) then
        return CONSTANTS["LOT_NULL"]
    elseif (#lotTable == 1) then
        return lotTable[1]
    end
    
    -- get current model LOT
    local curModel = self:GetVar("CurrentModel")
    local ranModel = curModel
    
    -- pick a random model until different
    while (ranModel == curModel) do
        local ran = math.random(1,#lotTable)
        ranModel = lotTable[ran]
    end
    
    return ranModel

end


--------------------------------------------------------------
-- Setup timer for firing
--------------------------------------------------------------
function SpawnModel(self, modelLOT)

	if (IsReady(self) == true) then

        -- get current model
        local childModel = getObjectByName(self,"childModel")
        if (childModel) then

            -- show change effect
			childModel::PlayFXEffect{ effectID = CONSTANTS["SCENE_1_STATUE_EFFECT_ID"], effectType = CONSTANTS["SCENE_1_STATUE_EFFECT_TYPE"] }
     
            -- get rid of the child
            GAMEOBJ:DeleteObject(childModel)
        
        end
        
		-- spawn the new model in
		local mypos = self:GetPosition().pos 
		mypos.x = mypos.x + CONSTANTS["SCENE_1_STATUE_OFFSET"].x
		mypos.y = mypos.y + CONSTANTS["SCENE_1_STATUE_OFFSET"].y
		mypos.z = mypos.z + CONSTANTS["SCENE_1_STATUE_OFFSET"].z
		
		RESMGR:LoadObject { objectTemplate = modelLOT, 
                            x = mypos.x, 
                            y = mypos.y, 
                            z = mypos.z,
                            owner = self }
                            
        -- store new model's LOT
        self:SetVar("CurrentModel", modelLOT) 
        
        -- set ready to false and create a cooldown
        SetIsReady(self, false)
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["SCENE_1_STATUE_COOLDOWN"], "ReadyCooldown",self )

	end
		
end

--------------------------------------------------------------
-- Get IsReady state
--------------------------------------------------------------
function IsReady(self)
	return self:GetVar("IsReady")
end


--------------------------------------------------------------
-- Set IsReady State
--------------------------------------------------------------
function SetIsReady(self, bReady)
	self:SetVar("IsReady", bReady)
end


--------------------------------------------------------------
-- Called when timers complete
--------------------------------------------------------------
onTimerDone = function(self, msg)

    if (msg.name == "ReadyCooldown") then
        SetIsReady(self,true)
    end
    
end    


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

	-- store spawn for use later
	storeObjectByName(self, "childModel", msg.childID)

end
