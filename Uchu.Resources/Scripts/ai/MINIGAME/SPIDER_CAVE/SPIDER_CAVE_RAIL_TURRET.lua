function onStartup(self)
    --print("starting up the rail turret")
    self:SetVar("finalDestination", false)
    self:SetVar("firstDestination", false)
    self:SetVar("isBuilt", false)
    self:SetVar("effectTime", 2)
end

function onRebuildNotifyState(self, msg)
    if (msg.iState == 2) then
        self:SetVar("isBuilt", true)
        --print("the rail turret is built")
    --[[else
        self:SetVar("isBuilt", false)
        print("the rail turret is not built")--]]
    end
end

function onNotifyObject(self, msg)
    --print("the turret got a message")
    if (msg.name == "fire") and (self:GetVar("isBuilt") == true) then
        --print("firing")
        self:PlayFXEffect{name = "moviespotlight", effectID = 503, effectType = "create"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( self:GetVar("effectTime")  , "effectTime", self )
    end
    if (msg.name == "firstPoint") and (self:GetVar("finalDestination") == true) then
        --self:SetVar("firstDestination", true)
        self:GoToWaypoint{iPathIndex = 2, bAllowPathingDirectionChange = false}
    elseif (msg.name == "firstPoint") then
        self:SetVar("firstDestination", true)
        self:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = false}
    elseif (msg.name == "lastPoint") and (self:GetVar("firstDestination") == true) then
        self:SetVar("finalDestination", true)
        self:GoToWaypoint{iPathIndex = 2, bAllowPathingDirectionChange = false}
    elseif (msg.name == "lastPoint") then
        self:SetVar("finalDestination", true)
        --print("final destination set to true")
    end
end

function onTimerDone(self, msg)
    self:StopFXEffect{name = "moviespotlight"}
end

function onArrived(self, msg)
    --print("at a waypoint" .. msg.wayPoint)
    if (msg.wayPoint == 2) then
        --print("at waypoint three")
        local friends = self:GetObjectsInGroup{group = "SpiderRailTurret", ignoreSpawners = true}.objects
        for i, object in pairs(friends) do
            if (object:GetLOT().objtemplate == 4956) then
                object:NotifyObject{name = "arrivedAtFinalWayPoint", ObjIDSender = self}
            end
        end
    end
end