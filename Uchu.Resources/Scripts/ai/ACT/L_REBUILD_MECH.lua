

--------------------------------------------------------------
-- Handles the object changing rebuild states
--------------------------------------------------------------
local SPAWN_SPEED = 8;
local MAX_SPAWNS = 6;
local ACTIVATE_RAD = 30;
local DEACTIVATE_RAD = 300;
function onRebuildNotifyState(self, msg)
    -- if someone just started rebuilding
    if (msg.iState == 3) then
       self:SetVar("numSpawns", 0); 
       GAMEOBJ:GetTimer():AddTimerWithCancel(  2 , "spawnTime", self );
    end
end

function onStartup(self)
    self:RebuildReset()
    self:DisplayRebuildActivator{ bShow = false }
    self:SetProximityRadius { radius = ACTIVATE_RAD, name = "build" };
    self:SetProximityRadius { radius = DEACTIVATE_RAD, name = "reset" };
end


function onTimerDone(self,msg)
    
    local spawnCount = self:GetVar("numSpawns");
    if(spawnCount < MAX_SPAWNS) then
        spawnCount = spawnCount + 1;
        self:SetVar("numSpawns", spawnCount);
        local pos = self:GetPosition().pos
        local randX = math.random(-10, 10);
        local randZ = math.random(-10, 10);
        
        RESMGR:LoadObject { objectTemplate = 2884  , x = pos.x  + randX, y =  pos.y , z = pos.z  + randZ, owner = self }
        GAMEOBJ:GetTimer():AddTimerWithCancel(  SPAWN_SPEED , "spawnTime", self );
    end
end

function onProximityUpdate(self,msg)
    local faction = msg.objId:GetFaction();
    if(faction and faction.faction == 1) then
        self:RebuildStart {userID = msg.objId}
        if(msg.name == "build" and msg.status == "ENTER") then
           msg.objId:SetAttr{ string = "maxlife", value = 10, ID = msg.objId}
           msg.objId:SetAttr{ string = "maxarmor", value = 5, ID = msg.objId}
           msg.objId:SetAttr{ string = "life", value = 10, ID = msg.objId}
           msg.objId:SetAttr{ string = "armor", value = 5, ID = msg.objId}
        elseif(msg.name == "reset" and msg.status == "LEAVE") then
            self:RebuildReset()
        end -- if name is build elseif name is reset
    end -- if faction is 1
end -- function
