--------------------------------------------------------------
-- Foot Race Goal script that the player will trigger.
-- updated mrb... 2/17/10
--------------------------------------------------------------

----------------------------------------------------------------
-- Called when the script starts up; this spawns in the appropriate 
-- objects for the foot race goals.
----------------------------------------------------------------
function onStartup(self)
    self:SetVisible{visible = false, fadeTime = 0}
    self:AddObjectToGroup{group = 'Goals_' .. self:GetVar('node_number')}
    self:SetVar('NextGoal', 1)
    GAMEOBJ:GetTimer():AddTimerWithCancel(0.5, "spawnGoals", self )
end

----------------------------------------------------------------
-- called when the script is shut down; destoys the child objects
----------------------------------------------------------------
function onShutdown(self, msg)
    killGoalPosts(self) -- if the script shuts down kill all of the child objects
end

----------------------------------------------------------------
-- called when the render component is loaded; plays the first effect.
----------------------------------------------------------------
function onChildRenderComponentReady(self,msg)
    if self:GetVar('node_number') == 1 then -- add delay to make sure the object is ready for an effect
        GAMEOBJ:GetTimer():AddTimerWithCancel(2, "Play_Goal_Post_Effect", self )   
    end
end

----------------------------------------------------------------
-- Spawns the goal posts and obsticles for the foot race
----------------------------------------------------------------
function spawnGoalPosts(self)
    local oPos = {pos = self:GetPosition().pos}
    local gOffset = self:GetVar("tGoalpostVars.goalpostOffset") -- self:GetObjectScale().scale -- scale doesn't work when loading objects
    local posOffset = -gOffset
    local oDir = self:GetObjectDirectionVectors()
    local numToSpawn = 2
    
    oPos.rot = self:GetRotation()          
    
    if self:GetVar("spawnObsticle") or self:GetVar("finalGoalObsticleLot") then -- spawn an obsticle or finish line
        numToSpawn = 3
    end
    
    local config = { {"groupID", "GoalPosts;Goals_" .. self:GetVar('node_number')} }
    
    for i=1, numToSpawn do -- spawn in the objects needed for this goal based on numToSpawn
        local spawnObj = self:GetVar("tGoalpostVars.goalpostLot")
        local dir = oDir.right
        
        if i == 3 then
            if self:GetVar("spawnObsticle") then
                spawnObj = self:GetVar("spawnObsticle")
                posOffset = 1 -- move the obsticle forward 1 unit so the player will collide with the obsticle first
            elseif self:GetVar("finalGoalObsticleLot") then
                spawnObj = self:GetVar("finalGoalObsticleLot")
                posOffset = -.5 -- move the finish line back .5 units so the player will collide with the trigger first
            end
            
            dir = oDir.forward
        end
        
        local newOffset = {x = oPos.pos.x + (dir.x * posOffset), y = oPos.pos.y, z = oPos.pos.z + (dir.z * posOffset)}
        
        if spawnObj and self and newOffset then
            -- load in the object
            RESMGR:LoadObject{ objectTemplate = spawnObj, x= newOffset.x, y= newOffset.y , z= newOffset.z, rw = oPos.rot.w, rx = oPos.rot.x, ry = oPos.rot.y, rz = oPos.rot.z, owner = self, configData = config} 
        end
        
        posOffset = gOffset
    end   
end

----------------------------------------------------------------
-- Plays the effect on the active goal
----------------------------------------------------------------
function PlayActiveEffect(self)
    if not self:GetVar("tGoalpostVars.nextEffectType") then return end
    
    local activeNode = self:GetVar('NextGoal')        
    local nextObjs = self:GetObjectsInGroup{ group = 'Goals_' .. activeNode, ignoreSpawners = true }.objects
        
    for k,v in ipairs(nextObjs) do -- find the correct goal and play the active effect
        if v:GetLOT().objtemplate == self:GetVar("tGoalpostVars.goalpostLot") then
            v:PlayFXEffect{name = "ActiveEffect", effectType = self:GetVar("tGoalpostVars.nextEffectType"), effectID = self:GetVar("tGoalpostVars.nextEffectID")}   
        end            
    end
end

----------------------------------------------------------------
-- Called when the player collides with the trigger volume; sends
-- messages to the foot race npc
----------------------------------------------------------------
function onCollisionPhantom(self, msg)    
    -- check to make sure we have the local player
    if msg.objectID:GetID() ~= GAMEOBJ:GetControlledID():GetID() then return end
    
    if not self:GetVar('isTriggered') then
        local nodeNum = self:GetVar('node_number')
        local activeNode = self:GetVar('NextGoal')
        
        if not activeNode then -- if we dont have an active node set it to the first one
            activeNode = 1
        end
        
        if nodeNum ~= activeNode then return end -- if we're not in the active node the skip everything else
    
        local nodeTotal = self:GetVar("total_spawner_nodes")
        local parentObj = self:GetParentObj().objIDParent
        
        if parentObj then -- fire the correct event to this objects parent
            if nodeNum == 1 then      
                parentObj:FireEvent{senderID = self, args = 'PlayerHitFirstGoal_' .. msg.objectID:GetID()}                    
            elseif nodeNum == nodeTotal then
                parentObj:FireEvent{senderID = self, args = 'PlayerWon_' .. msg.objectID:GetID()}                
            else
                parentObj:FireEvent{senderID = self, args = 'PlayerHitGoal_' .. msg.objectID:GetID()}
            end
        else
            print('*** missing parentObj ***')
        end
        
        local tGoalObjs = self:GetObjectsInGroup{ group = 'Goals', ignoreSpawners = true }.objects
        
        for k,v in ipairs(tGoalObjs) do -- set NextGoal on all of the goal objects
            v:SetVar('NextGoal', nodeNum + 1)
        end
                
        self:SetVar('isTriggered', true)        
        killGoalPosts(self)        
        PlayActiveEffect(self)
    end
end

----------------------------------------------------------------
-- Kills all of the child objects for this goal
----------------------------------------------------------------
function killGoalPosts(self)
    local nodeNum = self:GetVar('node_number')
    local killObjs = self:GetObjectsInGroup{ group = 'Goals_' .. nodeNum, ignoreSpawners = true }.objects
    
    for k,v in ipairs(killObjs) do
        --print('kill dem all ' .. k)
        if v:GetType().objType == "Smashables" or v:HasComponentType{iComponent = 7}.bHasComponent then -- if object is smashable call Die()
            v:Die()
        else -- no smashable so delete the object
            GAMEOBJ:DeleteObject( v )
        end
    end
end

----------------------------------------------------------------
-- Called when a timer is completed.
----------------------------------------------------------------
function onTimerDone(self, msg)  
	if msg.name == "spawnGoals" then -- spawn the goals
        spawnGoalPosts(self)  
	elseif msg.name == "Play_Goal_Post_Effect" then -- play the effect on the next active goal
        PlayActiveEffect(self)
	end	
end 