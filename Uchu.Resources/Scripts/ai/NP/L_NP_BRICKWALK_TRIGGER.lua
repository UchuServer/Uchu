--------------------------------------------------------------
-- (SERVER SIDE) Script for trigger to spawn bricks on a waypoint
--
-- Set the following config data:
-- markedAsPhantom  7:1
-- renderDisabled   7:1
-- spawn_path       0:pathname where pathname is the name of the path
-- waypoint         1:x where x is the waypoint number in the path
-- spawn_lot        0:LOT   where LOT is the LOT ID of the object to spawn
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')


--------------------------------------------------------------
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end


--------------------------------------------------------------
-- Get current number of player collisions
--------------------------------------------------------------
function GetCols(self)
    return tonumber(self:GetVar("NumCols"))
end


--------------------------------------------------------------
-- Add to current number of player collisions
--------------------------------------------------------------
function IncCols(self)
    local cols = self:GetVar("NumCols")
    cols = cols + 1
    self:SetVar("NumCols", cols)
end


--------------------------------------------------------------
-- Decrease current number of player collisions
--------------------------------------------------------------
function DecCols(self)
    local cols = self:GetVar("NumCols")
    cols = cols - 1
    if (cols < 0) then
        cols = 0
    end
    self:SetVar("NumCols", cols)
end    


--------------------------------------------------------------
-- Get the Spawn LOT Config data
--------------------------------------------------------------
function GetSpawnLOT(self)
    local lotID = self:GetVar("spawn_lot")
    return lotID
end    
    

--------------------------------------------------------------
-- Do the work to spawn/remove bricks
--------------------------------------------------------------
function DoBrickWork(self)

    if (GetCols(self) > 0) then
    
        print("spawn brick")
        
        -- if brick already exists, we are done
        local oldChild = getObjectByName(self, "ChildBrick")
        
        if (oldChild) and (oldChild:Exists()) then
            return
        end

        -- get waypoint from config data
        local wpNum = self:GetVar("waypoint")
        local pathName = self:GetVar("spawn_path")
        local lotID = GetSpawnLOT(self)
        
        if (wpNum) and (pathName) and (lotID) then

            print ("waypoint: " .. wpNum)

            -- spawn brick at waypoint on path
            --local objectPos = GAMEOBJ:GetWaypointPos( pathName, tonumber(wpNum) )
            
			local pathMsg = LEVEL:GetPathWaypoints(pathName)
			local trans
			if (tostring(type(pathMsg)) == "table") then
				trans = pathMsg[tonumber(wpNum)]

            -- load the object in the world
            RESMGR:LoadObject { objectTemplate = lotID,
                                x = trans.pos.x,
                                y = trans.pos.y,
                                z = trans.pos.z,
                                rw = trans.rot.w,
								rx = trans.rot.x,
								ry = trans.rot.y,
								rz = trans.rot.z,
                                owner = self }
			end
        end
        
    else
        -- if we have a child brick, remove it
        RemoveObject(self, "ChildBrick")
        print("remove brick")
    end
    
end


function RemoveObject(self, objName)

    local oldChild = getObjectByName(self, objName)
    if (oldChild ~= nil and oldChild:Exists()) then
        GAMEOBJ:DeleteObject(oldChild)
    end

end


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)
    self:SetVar("NumCols", 0)
end


function onCollisionPhantom(self, msg)
print("collision phantom")

    local faction = msg.objectID:GetFaction()
    --verify that we are only bouncing players
    if faction and faction.faction == 1 then
        IncCols(self)
    end
    
    DoBrickWork(self)        

end


function onOffCollisionPhantom(self, msg)

print ("collision off phantom")

    local faction = msg.senderID:GetFaction()
    --verify that we are only bouncing players
    if faction and faction.faction == 1 then
        DecCols(self)
    end
    
    DoBrickWork(self)

end


--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

    -- is this a goal object
	if tostring(msg.templateID) == GetSpawnLOT(self) then 

        -- remove old object, add new
        RemoveObject(self, "ChildBrick")
    	storeObjectByName(self, "ChildBrick", msg.childID)
    	storeParent(self, msg.childID)
    	
    end

end

    
    