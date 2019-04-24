local mobNum = 1
local spawnDist = 50
local playerFlag = 65

function onCollisionPhantom( self, msg )
	if msg.objectID:BelongsToFaction{factionID = 1}.bIsInFaction and not msg.objectID:GetFlag{iFlagID = playerFlag}.bFlag then
		
		local target = '|' .. msg.objectID:GetID()
		local oPos = {pos = self:GetPosition().pos}
        local posOffset = -5
        local oDir = msg.objectID:GetPlayerForward()
        
		if mobNum == 1 then 
		    posOffset = 10
		    spawnDist = 10
		end
		
        local newPos = {x = oPos.pos.x + (oDir.fwd.x * spawnDist), y = oPos.pos.y, z = oPos.pos.z + (oDir.fwd.z * spawnDist)} --+ (oDir.fwd.y * posOffset)
                    
        oPos.rot = self:GetRotation()          
        
		--print("************** Spawn Strombie **************")		
		local config = { {"wanderRadius", 35}, {"aggroRadius", 100}, {"tetherRadius", 150}, {"no_timed_spawn", true}, {"tempTarget", target}}
        for i=1, mobNum do
            local newOffset = {x = newPos.x + (posOffset/i), y = newPos.y, z = newPos.z + posOffset}
            RESMGR:LoadObject{ objectTemplate = 4712, x= newOffset.x, y= newOffset.y , z= newOffset.z, rw = oPos.rot.w, rx = oPos.rot.x, ry = oPos.rot.y, rz = oPos.rot.z, owner = self, configData = config} --
            posOffset = posOffset + 5 
        end   
        
        msg.objectID:SetFlag{iFlagID = playerFlag, bFlag = true}
	    
	end

end

function onChildLoaded( self, msg )
    local stromMob = msg.childID
    local tempTarget = stromMob:GetVar("tempTarget" )
    local target = GAMEOBJ:GetObjectByID(tempTarget)
    --print(stromMob:GetName().name .. ' should attack ' .. target:GetName().name)
    --stromMob:UnsetProximityRadius{name = "aggroRadius"}
    --stromMob:SetProximityRadius{radius = 100, name = "aggroRadius"}
    stromMob:AddThreatRating{newThreatObjID = target, ThreatToAdd = 500}  
end

